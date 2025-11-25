using System.Net;
using Backend_Net.Application.Common.Extensions;
using Backend_Net.Application.Common.Helpers;
using Backend_Net.Application.Common.Interfaces;
using Backend_Net.Application.Common.Interfaces.Repositories;
using Backend_Net.Application.Constants;
using Backend_Net.Application.Services.Signature;
using Backend_Net.Domain.Entities;
using Backend_Net.Domain.Enums;
using Backend_Net.Infrastructure.Options;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Backend_Net.Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, CreateOrderResponse>
{
    private readonly ILogger<CreateOrderHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISignatureService _signatureService;
    private readonly AppOptions _appOptions;

    public CreateOrderHandler
    (
        ILogger<CreateOrderHandler> logger,
        IUnitOfWork unitOfWork,
        ISignatureService signatureService,
        IOptions<AppOptions> appOptions
    )
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _signatureService = signatureService;
        _appOptions = appOptions.Value;
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
                .Select(x => new
                {
                    x.Id,
                    x.ApiKey,
                    x.SecretEncrypted,
                    x.Status,
                    TenantId = x.Tenant.Id,
                    TenantIsActive = x.Tenant.IsActive
                })
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);
            if (credential == null)
            {
                _logger.LogWarning("{Fn} Tenant not found: {ApiKey}", functionName, request.ApiKey);
                response
                    .WithSuccess(false)
                    .WithMessage(ErrorCode.TEN_ERR_001)
                    .WithStatus(HttpStatusCode.NotFound);

                return response;
            }

            if (!credential.TenantIsActive)
            {
                _logger.LogWarning("{Fn} Tenant inactive: {ApiKey}", functionName, request.ApiKey);
                response
                    .WithSuccess(false)
                    .WithMessage(ErrorCode.TEN_ERR_002)
                    .WithStatus(HttpStatusCode.Forbidden);
                return response;
            }

            var tenantSecret = CryptographyHelper.Decrypt(credential.SecretEncrypted, _appOptions.ClientSecret);
            var serverSignature = _signatureService.CreateSignature(payload.ToDictionary(), tenantSecret);
            if (string.IsNullOrWhiteSpace(serverSignature) ||
                !serverSignature.Equals(request.Signature, StringComparison.Ordinal))
            {
                _logger.LogWarning("{Fn} Invalid signature", functionName);
                response
                    .WithSuccess(false)
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
                    .WithSuccess(false)
                    .WithMessage(ErrorCode.ORD_ERR_001)
                    .WithStatus(HttpStatusCode.Conflict);
                return response;
            }

            var now = DateTime.UtcNow;

            var paymentTransaction = new PaymentTransaction
            {
                TenantId = credential.TenantId,
                UserId = payload.UserId,
                OrderId = payload.ReferenceId,
                PaymentMethodId = null,
                CurrencyCode = null,
                Amount = payload.Amount,
                FeeAmount = 0,
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
            await _unitOfWork.SaveAsync();

            var encryptedPath = CryptographyHelper.Encrypt(ApplicationConstant.PaymentUrl, _appOptions.ClientSecret);
            var paymentUrl = string.Format(ApplicationConstant.PaymentUrl, _appOptions.HostingUrl, encryptedPath);
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
        }

        return response;
    }

    #endregion
}