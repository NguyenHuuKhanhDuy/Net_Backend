using System.Net;
using Backend_Net.Application.Common.Extensions;
using Backend_Net.Application.Common.Helpers;
using Backend_Net.Application.Common.Interfaces;
using Backend_Net.Application.Common.Interfaces.Azure;
using Backend_Net.Application.Common.Interfaces.MassTransit;
using Backend_Net.Application.Common.Interfaces.Repositories;
using Backend_Net.Application.Constants;
using Backend_Net.Application.Models.Dtos;
using Backend_Net.Application.Options;
using Backend_Net.Application.Services.PaymentToken;
using Backend_Net.Application.Services.Signature;
using Backend_Net.Domain.Entities;
using Backend_Net.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Extensions;
using Shared.Helpers;
using Shared.MassTransit.Contracts.Queues;
using Shared.MassTransit.IntegrationEvents;

namespace Backend_Net.Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, CreateOrderResponse>
{
    private readonly ILogger<CreateOrderHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISignatureService _signatureService;
    private readonly AppOptions _appOptions;
    private readonly IPaymentTokenService _paymentTokenService;
    private readonly ISecretService _secretService;

    public CreateOrderHandler
    (
        ILogger<CreateOrderHandler> logger,
        IUnitOfWork unitOfWork,
        ISignatureService signatureService,
        IOptions<AppOptions> appOptions,
        IPaymentTokenService paymentTokenService,
        ISecretService secretService
    )
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _signatureService = signatureService;
        _appOptions = appOptions.Value;
        _paymentTokenService = paymentTokenService;
        _secretService = secretService;
    }

    #region Implementation of IRequestHandler<in CreateOrderCommand, CreateOrderResponse>

    public async Task<CreateOrderResponse> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var payload = request.Payload;
        var functionName = $"{nameof(CreateOrderHandler)} TenantId={request.ApiKey} =>";
        _logger.LogInformation("{Fn} Request: {@Payload}", functionName, payload);

        var response = new CreateOrderResponse
        {
            Success = false,
            StatusCode = HttpStatusCode.InternalServerError
        };

        try
        {
            var credential = await _unitOfWork.TenantCredential
                .Where(x => x.ApiKey == request.ApiKey)
                .Include(x => x.Tenant)
                .ThenInclude(x => x.TenantPaymentMethodCurrencies)
                .Select(x => new
                {
                    x.Id,
                    x.ApiKey,
                    x.Status,
                    TenantId = x.Tenant.Id,
                    TenantIsActive = x.Tenant.IsActive,
                    Currencies = x.Tenant.TenantPaymentMethodCurrencies
                        .Where(tpmc => tpmc.IsEnabled)
                        .Select(tpmc => tpmc.CurrencyCode)
                        .ToList()
                })
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);
            if (credential == null)
            {
                _logger.LogWarning("{Fn} Tenant not found: {ApiKey}", functionName, request.ApiKey);
                response
                    .WithMessage(ErrorCode.TEN_ERR_001)
                    .WithStatus(HttpStatusCode.NotFound);

                return response;
            }
            
            if (!credential.TenantIsActive)
            {
                _logger.LogWarning("{Fn} Tenant inactive: {ApiKey}", functionName, request.ApiKey);
                response
                    .WithMessage(ErrorCode.TEN_ERR_002)
                    .WithStatus(HttpStatusCode.Forbidden);
                return response;
            }

            if (!credential.Currencies.Contains(payload.Currency))
            {
                _logger.LogInformation("{Fn} Currency not supported: {Currency}", functionName, payload.Currency);
                response
                    .WithMessage(ErrorCode.CUR_ERR_001)
                    .WithStatus(HttpStatusCode.BadRequest);
                return response;
            }
            
            var currency = await _unitOfWork.Currency.GetAll()
                .AsNoTracking()
                .Select(x => new { Code = x.Code })
                .FirstOrDefaultAsync(x => x.Code == payload.Currency, cancellationToken);
            if (currency == null)
            {
                _logger.LogWarning("{Fn} Currency not found: {Currency}", functionName, payload.Currency);
                response
                    .WithMessage(ErrorCode.CUR_ERR_001)
                    .WithStatus(HttpStatusCode.BadRequest);
                return response;
            }

            var credentialSecret = await _secretService.GetSecretAsync(credential.Id.ToString(), cancellationToken);
            if (string.IsNullOrWhiteSpace(credentialSecret))
            {
                _logger.LogError("{Fn} Credential secret not found: {CredentialId}", functionName, credential.Id);
                response
                    .WithMessage(ErrorCode.EXH_ERR_002);
                return response;
            }
            
            var serverSignature = _signatureService.CreateSignature(payload.ToDictionary(), credentialSecret);
            if (string.IsNullOrWhiteSpace(serverSignature) ||
                !serverSignature.Equals(request.Signature, StringComparison.Ordinal))
            {
                _logger.LogWarning("{Fn} Invalid signature", functionName);
                response
                    .WithMessage(ErrorCode.ORD_ERR_002)
                    .WithStatus(HttpStatusCode.BadRequest);
                return response;
            }

            var existed = await _unitOfWork.PaymentTransaction
                .GetAll()
                .AsNoTracking()
                .AnyAsync(x =>
                        x.TenantId == credential.TenantId &&
                        x.OrderId == payload.ReferenceId,
                    cancellationToken);
            if (existed)
            {
                _logger.LogWarning("{Fn} OrderId exists: {OrderId}", functionName, payload.ReferenceId);
                response
                    .WithMessage(ErrorCode.ORD_ERR_001)
                    .WithStatus(HttpStatusCode.Conflict);
                return response;
            }

            var now = DateTime.UtcNow;
            var paymentTransaction = new PaymentTransaction
            {
                TenantCredentialId = credential.Id,
                TenantId = credential.TenantId,
                UserId = payload.UserId,
                OrderId = payload.ReferenceId,
                PaymentMethodId = null,
                TenantCurrencyCode = currency.Code,
                Amount = payload.Amount,
                Fee = null,
                PayerFeeType = null,
                NetAmount = payload.Amount,
                Status = PaymentStatus.Created,
                ProviderTxnId = null,
                ProviderRawReq = null,
                ProviderRawRes = null,
                ReturnUrl = payload.ReturnUrl,
                CallbackUrl = payload.CallbackUrl,
                CallbackData = payload.CallbackData != null
                    ? JsonHelper.Serialize(payload.CallbackData)
                    : null,
                CreatedAt = now
            };

            await _unitOfWork.PaymentTransaction.Add(paymentTransaction);
            await _unitOfWork.SaveAsync(cancellationToken);

            var expireAt = now.AddMinutes(_appOptions.TokenExpirationInMinutes);
            var token = _paymentTokenService.GeneratePaymentTokenAsync(paymentTransaction.Id, expireAt, cancellationToken: cancellationToken);
            var paymentUrl = string.Format(ApplicationConstant.PaymentUrl, _appOptions.HostingUrl, token);
            response.Data = new CreateOrderData
            {
                Id = paymentTransaction.Id,
                PaymentUrl = paymentUrl,
                Fee = 0
            };

            response
                .WithSuccess(true)
                .WithStatus(HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            ex.LogError(_logger, functionName);
            response
                .WithMessage(ErrorCode.EXH_ERR_001);
        }

        return response;
    }

    #endregion
}