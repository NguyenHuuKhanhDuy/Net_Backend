using System.Net;
using Backend_Net.Application.Common.Interfaces.Repositories;
using Backend_Net.Application.Constants;
using Backend_Net.Application.Services.PaymentToken;
using Backend_Net.Application.Services.Rate;
using Backend_Net.Domain.Entities;
using Backend_Net.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Extensions;

namespace Backend_Net.Application.Features.Orders.Commands.ConfirmOrder;

public class ConfirmOrderHandler : IRequestHandler<ConfirmOrderCommand, ConfirmOrderResponse>
{
    private readonly ILogger<ConfirmOrderHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPaymentTokenService _paymentTokenService;
    private readonly IRateService _rateService;

    public ConfirmOrderHandler
    (
        ILogger<ConfirmOrderHandler> logger,
        IUnitOfWork unitOfWork,
        IPaymentTokenService paymentTokenService,
        IRateService rateService
    )
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _paymentTokenService = paymentTokenService;
        _rateService = rateService;
    }

    #region Implementation of IRequestHandler<in ConfirmOrderCommand, ConfirmOrderResponse>

    public async Task<ConfirmOrderResponse> Handle(ConfirmOrderCommand request, CancellationToken cancellationToken)
    {
        var payload = request.Payload;
        var functionName = $"{nameof(ConfirmOrderHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new ConfirmOrderResponse { Success = false, StatusCode = HttpStatusCode.InternalServerError };

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

            var transaction = await _unitOfWork.PaymentTransaction
                .Where(x => x.Id == paymentToken.PaymentTransactionId)
                .Include(x => x.TenantCurrency)
                .Select(x => new
                {
                    OrderId = x.OrderId,
                    Description = x.Description,
                    Amount = x.Amount,
                    UserId = x.UserId,
                    Status = x.Status,
                    TenantId = x.TenantId,
                    TenantCurrency = new Currency
                    {
                        Code = x.TenantCurrency.Code,
                        IsGlobal = x.TenantCurrency.IsGlobal,
                        Type = x.TenantCurrency.Type
                    }
                })
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);
            if (transaction == null)
            {
                _logger.LogError($"{functionName} payment transaction not found for id: {paymentToken.PaymentTransactionId}");
                response.WithMessage(ErrorCode.PMT_ERR_001);
                return response;
            }
            
            if (transaction.Status != PaymentStatus.Created)
            {
                _logger.LogInformation("{FunctionName:l} payment transaction status is not valid: {Status:l}", functionName, transaction.Status);
                response
                    .WithStatus(HttpStatusCode.BadRequest)
                    .WithMessage(ErrorCode.PMT_ERR_002);
                return response;
            }
            
            var country = await _unitOfWork.Country.GetAll()
                .FirstOrDefaultAsync(x => x.Id == payload.CountryId, cancellationToken);
            if (country == null)
            {
                _logger.LogError($"{functionName} country not found for id: {payload.CountryId}");
                response.WithMessage(ErrorCode.PMM_ERR_001);
                return response;
            }

            var tenantPaymentMethod = await _unitOfWork.TenantPaymentMethod.GetPaymentMethodByTenantIdAsync
            (
                transaction.TenantId,
                country,
                payload.MethodId,
                transaction.TenantCurrency,
                cancellationToken
            );
            if (tenantPaymentMethod == null)
            {
                _logger.LogError($"{functionName} payment method not found for tenant id: {paymentToken.PaymentTransactionId}, country id: {payload.CountryId}, payment id: {payload.MethodId}");
                response.WithMessage(ErrorCode.PMM_ERR_001);
                return response;
            }

            var paymentMethod = tenantPaymentMethod.PaymentMethod;
            var rate = 1m;
            var userCurrency = country.CurrencyCode;
            if
            (
                paymentMethod.Type == PaymentMethodType.Global
                && !paymentMethod.PaymentMethodCurrencies.Any(x => x.CurrencyCode == userCurrency)
            )
            {
                userCurrency = ApplicationConstant.Currencies.USD;
            }
            
            if (userCurrency != transaction.TenantCurrency.Code)
            {
                var exchangeRate = _rateService.Convert(transaction.TenantCurrency.Code, userCurrency);
                if (!exchangeRate.HasValue)
                {
                    _logger.LogWarning($"{functionName} failed to get exchange rate from {transaction.TenantCurrency.Code} to {country.CurrencyCode}");
                    response.WithMessage(ErrorCode.PMM_ERR_001);
                    return response;
                }
                
                rate = exchangeRate.Value;
            }
            
            var equivalentAmount = transaction.Amount * rate;
            response.Data = new ConfirmOrderData
            {
                Amount = transaction.Amount,
                Description = transaction.Description,
                Status = transaction.Status,
                UserId = transaction.UserId,
                OrderNumber = transaction.OrderId,
                PaymentMethodType = paymentMethod.PaymentMethodType,
                TenantCurrency = transaction.TenantCurrency.Code,
                UserCurrency = userCurrency,
                EquivalentAmount = equivalentAmount,
            };

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
    
    private decimal CalculateFee(decimal fee, FeeType feeType, decimal amount)
    {
        return feeType switch
        {
            FeeType.Percentage => (fee / 100) * amount,
            FeeType.Fixed => fee,
            _ => 0m
        };
    }

    #endregion
}