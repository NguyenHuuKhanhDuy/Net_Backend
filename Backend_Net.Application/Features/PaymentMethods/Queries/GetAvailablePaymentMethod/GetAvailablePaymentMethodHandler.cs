using System.Net;
using Backend_Net.Application.Common.Extensions;
using Backend_Net.Application.Common.Helpers;
using Backend_Net.Application.Common.Interfaces.Repositories;
using Backend_Net.Application.Models.Dtos;
using Backend_Net.Application.Services.PaymentToken;
using Backend_Net.Domain.Entities;
using Backend_Net.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Extensions;

namespace Backend_Net.Application.Features.PaymentMethods.Queries.GetAvailablePaymentMethod;

public class
    GetAvailablePaymentMethodHandler : IRequestHandler<GetAvailablePaymentMethodQuery,
    GetAvailablePaymentMethodResponse>
{
    private readonly ILogger<GetAvailablePaymentMethodHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPaymentTokenService _paymentTokenService;

    public GetAvailablePaymentMethodHandler
    (
        ILogger<GetAvailablePaymentMethodHandler> logger,
        IUnitOfWork unitOfWork,
        IPaymentTokenService paymentTokenService
    )
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _paymentTokenService = paymentTokenService;
    }

    #region Implementation of IRequestHandler<in GetAvailablePaymentMethodQuery, GetAvailablePaymentMethodResponse>

    public async Task<GetAvailablePaymentMethodResponse> Handle(GetAvailablePaymentMethodQuery request,
        CancellationToken cancellationToken)
    {
        var payload = request.Payload;
        var functionName = $"{nameof(GetAvailablePaymentMethodHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new GetAvailablePaymentMethodResponse
            { Success = false, StatusCode = HttpStatusCode.InternalServerError };

        try
        {
            var extractedToken = _paymentTokenService.ExtractPaymentTokenAsync(payload.Token, cancellationToken);
            if (!extractedToken.Success)
            {
                _logger.LogWarning("{FunctionName} failed to extract payment token: {ErrorMessage}", functionName, extractedToken.ErrorMessage);
                response.ErrorMessage = extractedToken.ErrorMessage;
                response.ErrorMessageCode = extractedToken.ErrorMessageCode;
                response.WithStatus(HttpStatusCode.BadRequest);
                return response;
            }

            var paymentToken = extractedToken.Data;
            if (paymentToken is null || paymentToken.ExpireAt < DateTime.UtcNow)
            {
                _logger.LogWarning("{Fn:l} Token is expired", functionName);
                response
                    .WithMessage(ErrorCode.INV_ERR_001)
                    .WithStatus(HttpStatusCode.Unauthorized);
                return response;
            }

            var paymentTransaction = await _unitOfWork.PaymentTransaction
                .Where(x => x.Id == paymentToken.PaymentTransactionId)
                .Include(x => x.TenantCurrency)
                .Select(x => new
                {
                    TenantId = x.TenantId,
                    Currency = new Currency
                    {
                        Code = x.TenantCurrency.Code, 
                        IsGlobal = x.TenantCurrency.IsGlobal
                    }
                })
                .FirstOrDefaultAsync(cancellationToken);
            if (paymentTransaction == null)
            {
                _logger.LogError($"{functionName} payment transaction not found for id: {paymentToken.PaymentTransactionId}");
                response.WithMessage(ErrorCode.PMT_ERR_001);
                return response;
            }

            var tenantPaymentMethods =
                await _unitOfWork.TenantPaymentMethod.GetPaymentMethodsByTenantIdAsync(paymentTransaction.TenantId,
                    payload.CountryId, paymentTransaction.Currency, cancellationToken);
            response.Data = tenantPaymentMethods.Select(x => new GetAvailablePaymentMethodData
            {
                Id = x.Id,
                Name = x.PaymentMethod.DisplayName,
                BackgroundUrl = x.PaymentMethod.BackgroundUrl
            }).ToList();

            response
                .WithSuccess(true)
                .WithStatus(HttpStatusCode.OK);
        }
        catch (Exception e)
        {
            e.LogError(_logger, functionName);
        }

        return response;
    }

    #endregion
}