using System.Globalization;
using System.Net.Http.Json;
using System.Text;
using Backend_Net.Application.Common.Extensions;
using Backend_Net.Application.Common.Helpers;
using Backend_Net.Application.Common.Interfaces.Azure;
using Backend_Net.Application.Common.Interfaces.Repositories;
using Backend_Net.Application.Constants;
using Backend_Net.Application.Models.Dtos;
using Backend_Net.Application.Options;
using Backend_Net.Application.Services.Signature;
using Backend_Net.Domain.Entities;
using Backend_Net.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Extensions;
using Shared.Helpers;

namespace Backend_Net.Application.Services.Webhook;

public class WebhookService : IWebhookService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly HttpClient _http;
    private readonly ILogger<WebhookService> _logger;
    private readonly ISignatureService _signatureService;
    private readonly AppOptions _appOptions;
    private readonly ISecretService _secretService;

    public WebhookService
    (
        IUnitOfWork unitOfWork,
        IHttpClientFactory httpClientFactory,
        ILogger<WebhookService> logger,
        ISignatureService signatureService,
        IOptions<AppOptions> appOptions,
        ISecretService secretService
    )
    {
        _unitOfWork = unitOfWork;
        _http = httpClientFactory.CreateClient();
        _logger = logger;
        _signatureService = signatureService;
        _appOptions = appOptions.Value;
        _secretService = secretService;
    }
    
    public async Task<Guid> QueueWebhookAsync(Guid paymentTransactionId, CancellationToken cancellationToken)
    {
        var fun = $"{nameof(WebhookService)} paymentTransactionId = {paymentTransactionId} =>";
        _logger.LogInformation(fun);
        
        var paymentTransaction = await _unitOfWork.PaymentTransaction
            .Where(x => x.Id == paymentTransactionId)
            .Include(x => x.TenantCredential)
            .Select(x => new PaymentTransaction
            {
                Id = x.Id,
                OrderId = x.OrderId,
                Amount = x.Amount,
                TenantCurrencyCode = x.TenantCurrencyCode,
                UserId = x.UserId,
                CallbackData = x.CallbackData,
                Status = x.Status,
                ReturnUrl = x.ReturnUrl,
                CallbackUrl = x.CallbackUrl,
                CreatedAt = x.CreatedAt,
                TenantCredential = new TenantCredential
                {
                    Id = x.TenantCredential.Id,
                    ApiKey = x.TenantCredential.ApiKey
                },
                TenantId = x.TenantId,
            })
            .FirstOrDefaultAsync(cancellationToken);
        if (paymentTransaction is null)
        {
            return Guid.Empty;
        }

        var payload = new WebhookDto
        {
            Id = paymentTransaction.Id,
            ReferenceId = paymentTransaction.OrderId,
            Amount = paymentTransaction.Amount.ToString(CultureInfo.InvariantCulture),
            Currency = paymentTransaction.TenantCurrencyCode,
            UserId = paymentTransaction.UserId,
            CallbackData = paymentTransaction.CallbackData,
            Status = ((int)paymentTransaction.Status).ToString(),
            ReturnUrl = paymentTransaction.ReturnUrl,
            CallbackUrl = paymentTransaction.CallbackUrl,
            CreatedAt = paymentTransaction.CreatedAt.ToString("O"),
        };

        var secret = await _secretService.GetSecretAsync(paymentTransaction.TenantCredential.Id.ToString(), cancellationToken);
        if (string.IsNullOrEmpty(secret))
        {
            _logger.LogInformation("{FunctionName:l} Secret not found for TenantCredentialId: {TenantCredentialId}", fun, paymentTransaction.TenantCredential.Id);
            return Guid.Empty;
        }
        
        var signature = _signatureService.CreateSignature(payload.ToDictionary(), secret);
        var evt = new WebhookEvent
        {
            Id = Guid.NewGuid(),
            TenantId = paymentTransaction.TenantId,
            TransactionId = paymentTransaction.Id,
            CallbackUrl = paymentTransaction.CallbackUrl,
            EventType = "PAYMENT_TRANSACTION_UPDATED",
            Payload = JsonHelper.Serialize(payload),
            CreatedAt = DateTime.UtcNow,
            Signature = signature,
            ApiKey = paymentTransaction.TenantCredential.ApiKey,
        };
        
        await _unitOfWork.WebhookEvent.Add(evt);
        await _unitOfWork.SaveAsync(cancellationToken);
        
        var delivery = new WebhookDelivery
        {
            Id = Guid.NewGuid(),
            EventId = evt.Id,
            Attempt = 1,
            Status = WebhookDeliveryStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
        
        await _unitOfWork.WebhookDelivery.Add(delivery);
        await _unitOfWork.SaveAsync(cancellationToken);
        
        return evt.Id;
    }

    public async Task ExecuteWebhookBatchAsync(List<Guid> webhookDeliveryIds, CancellationToken cancellationToken)
    {
        var idList = webhookDeliveryIds.Distinct().ToList();

        if (idList.Count == 0)
            return;

        var deliveries = await _unitOfWork.WebhookDelivery
            .Where(x => idList.Contains(x.Id))
            .GroupBy(x => x.EventId)
            .Select(g => g.OrderByDescending(d => d.Attempt).First())
            .ToListAsync(cancellationToken);
        
        var eventIds = deliveries.Select(x => x.EventId).Distinct().ToList();
        var events = await _unitOfWork.WebhookEvent
            .GetAll()
            .Where(x => eventIds.Contains(x.Id))
            .ToListAsync(cancellationToken);
        
        var tasks = new List<Task>();
        foreach (var delivery in deliveries)
        {
            var evt = events.FirstOrDefault(x => x.Id == delivery.EventId);
            if (evt is null)
                continue;

            tasks.Add(SendSingleWebhookAsync(evt, delivery, cancellationToken));
        }

        await Task.WhenAll(tasks);
        await _unitOfWork.SaveAsync(cancellationToken);
    }
    
    private async Task<bool> SendSingleWebhookAsync(WebhookEvent evt, WebhookDelivery delivery, CancellationToken cancellationToken)
    {
        var func = $"{nameof(WebhookService)} evt = {evt.Id} =>";

        delivery.SentAt = DateTime.UtcNow;
        delivery.RequestBody = evt.Payload;

        try
        {
            var request = new HttpRequestMessage(HttpMethod.Post, evt.CallbackUrl)
            {
                Content = new StringContent(evt.Payload, Encoding.UTF8, "application/json")
            };

            request.Headers.Add(ApplicationConstant.Headers.ApiKey, evt.ApiKey);
            request.Headers.Add(ApplicationConstant.Headers.Signature, evt.Signature);

            var httpResponse = await _http.SendAsync(request, cancellationToken);

            delivery.HttpStatus = (int)httpResponse.StatusCode;
            delivery.ResponseBody = await httpResponse.Content.ReadAsStringAsync(cancellationToken);

            if (httpResponse.IsSuccessStatusCode)
            {
                delivery.Status = WebhookDeliveryStatus.Success;
                return true;
            }

            delivery.Status = WebhookDeliveryStatus.Failed;
            delivery.ErrorMessage = $"HTTP {(int)httpResponse.StatusCode}";
            ScheduleRetry(delivery);

            return false;
        }
        catch (Exception ex)
        {
            delivery.Status = WebhookDeliveryStatus.Failed;
            delivery.ErrorMessage = ex.Message;

            ScheduleRetry(delivery);

            ex.LogError(_logger, func);
            return false;
        }
    }
    
    private static void ScheduleRetry(WebhookDelivery d)
    {
        if (d.Attempt >= d.MaxAttempt)
        {
            d.RetryAt = null;
            return;
        }

        d.Attempt++;
        d.RetryAt = DateTime.UtcNow + d.GetBackoffDelay(d.Attempt);
    }
}