using System.Text.Json.Serialization;
using Backend_Net.Application.Common.Interfaces.PaymentProvider;
using Backend_Net.Application.Common.Interfaces.Repositories;
using Backend_Net.Application.Services.Webhook;
using Backend_Net.Domain.Enums;
using Backend_Net.Domain.Models.Stripe;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Extensions;
using Shared.Helpers;

namespace Backend_Net.Application.Features.PaymentProvider.Stripe.Command.HandleStripeCallbackPayment;

public class HandleStripeCallbackPaymentHandler : IRequestHandler<HandleStripeCallbackPaymentCommand>
{
    private readonly ILogger<HandleStripeCallbackPaymentHandler> _logger;
    private readonly IStripeService _stripeService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebhookService _webhookService;

    public HandleStripeCallbackPaymentHandler
    (
        ILogger<HandleStripeCallbackPaymentHandler> logger,
        IStripeService stripeService,
        IUnitOfWork unitOfWork,
        IWebhookService webhookService
    )
    {
        _logger = logger;
        _stripeService = stripeService;
        _unitOfWork = unitOfWork;
        _webhookService = webhookService;
    }

    #region Implementation of IRequestHandler<in HandleStripeCallbackPaymentCommand, HandleStripeCallbackPaymentResponse>

    public async Task Handle(HandleStripeCallbackPaymentCommand request, CancellationToken cancellationToken)
    {
        var payload = request.Payload;
        var functionName = $"{nameof(HandleStripeCallbackPaymentHandler)} CorrelationId = {payload.CorrelationId} =>";
        _logger.LogInformation("{functionName:l} Payload = {payload:l}", functionName, payload);

        try
        {
            var isValid = await _stripeService.IsValidWebhook(new HandleStripeCallbackPaymentRequest
            {
                JsonData = payload.JsonData,
                StripeSignature = payload.StripeSignature,
            }, cancellationToken);
            if (!isValid)
            {
                _logger.LogWarning("{functionName:l} Invalid Stripe webhook signature", functionName);
                return;
            }
            
            var stripePayload = JsonHelper.Deserialize<StripeWebhookPayload>(payload.JsonData);
            if (stripePayload == null)
            {
                _logger.LogWarning("{functionName:l} Unable to deserialize Stripe webhook payload", functionName);
                return;
            }
            
            var transactionId = stripePayload.Data.Object.Metadata["transactionId"];
            if (string.IsNullOrEmpty(transactionId))
            {
                _logger.LogWarning("{functionName:l} transactionId not found in Stripe webhook metadata", functionName);
                return;
            }

            var paymentTransaction = await _unitOfWork.PaymentTransaction
                .Where(x => x.Id == Guid.Parse(transactionId))
                .FirstOrDefaultAsync(cancellationToken);
            if (paymentTransaction is null)
            {
                _logger.LogWarning("{functionName:l} PaymentTransaction not found for Id = {transactionId:l}", functionName, transactionId);
                return;
            }

            paymentTransaction.Status = PaymentStatus.Success;
            paymentTransaction.UpdatedAt = DateTime.UtcNow;
            paymentTransaction.ProviderRawRes = payload.JsonData;
            await _unitOfWork.SaveAsync(cancellationToken);
            await _webhookService.QueueWebhookAsync(paymentTransaction.Id, cancellationToken);
        }
        catch (Exception ex)
        {
            ex.LogError(_logger, functionName);
        }
    }
    
    private class StripeWebhookPayload
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        
        [JsonPropertyName("type")]
        public string EventType { get; set; }
        
        [JsonPropertyName("data")]
        public StripeData Data { get; set; }
    }

    private class StripeData
    {
        [JsonPropertyName("object")]
        public StripeObject Object { get; set; }
    }

    private class StripeObject
    {
        [JsonPropertyName("metadata")]
        public Dictionary<string, string> Metadata { get; set; }
    }

    #endregion
}