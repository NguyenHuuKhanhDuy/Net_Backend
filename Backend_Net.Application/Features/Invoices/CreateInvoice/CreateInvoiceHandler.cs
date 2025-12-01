using System.Net;
using Backend_Net.Application.Common.Interfaces.PaymentProvider;
using Backend_Net.Application.Common.Interfaces.Repositories;
using Backend_Net.Application.Constants;
using Backend_Net.Application.Options;
using Backend_Net.Application.Services.PaymentToken;
using Backend_Net.Application.Services.Rate;
using Backend_Net.Domain.Enums;
using Backend_Net.Domain.Models.Stripe;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Extensions;

namespace Backend_Net.Application.Features.Invoices.CreateInvoice;

public class CreateInvoiceHandler : IRequestHandler<CreateInvoiceCommand, CreateInvoiceResponse>
{
    private readonly ILogger<CreateInvoiceHandler> _logger;
    private readonly IPaymentTokenService _paymentTokenService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRateService _rateService;
    private readonly IStripeService _stripeService;
    private readonly AppOptions _appOptions;

    public CreateInvoiceHandler
    (
        ILogger<CreateInvoiceHandler> logger,
        IPaymentTokenService paymentTokenService,
        IUnitOfWork unitOfWork,
        IRateService rateService,
        IStripeService stripeService,
        IOptions<AppOptions> appOptions
    )
    {
        _logger = logger;
        _paymentTokenService = paymentTokenService;
        _unitOfWork = unitOfWork;
        _rateService = rateService;
        _stripeService = stripeService;
        _appOptions = appOptions.Value;
    }

    #region Implementation of IRequestHandler<in CreateInvoiceCommand, CreateInvoiceResponse>

    public async Task<CreateInvoiceResponse> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
    {
        var payload = request.Payload;
        var functionName = $"{nameof(CreateInvoiceHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new CreateInvoiceResponse { Success = false, StatusCode = HttpStatusCode.InternalServerError };

        try
        {
            var extractedToken = _paymentTokenService.ExtractPaymentTokenAsync(payload.Token, cancellationToken);
            if (!extractedToken.Success)
            {
                _logger.LogWarning("{FunctionName} failed to extract payment token: {ErrorMessage}", functionName,
                    extractedToken.ErrorMessage);
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
                .FirstOrDefaultAsync(cancellationToken);
            if (transaction == null)
            {
                _logger.LogError(
                    $"{functionName} payment transaction not found for id: {paymentToken.PaymentTransactionId}");
                response.WithMessage(ErrorCode.PMT_ERR_001);
                return response;
            }

            if (transaction.Status != PaymentStatus.Created)
            {
                _logger.LogInformation("{FunctionName:l} payment transaction status is not valid: {Status:l}",
                    functionName, transaction.Status);
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
                _logger.LogError(
                    $"{functionName} payment method not found for tenant id: {paymentToken.PaymentTransactionId}, country id: {payload.CountryId}, payment id: {payload.MethodId}");
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
            var expireAt = transaction.CreatedAt.AddMinutes(_appOptions.TokenExpirationInMinutes);
            var token = _paymentTokenService.GeneratePaymentTokenAsync(transaction.Id, expireAt,
                cancellationToken: cancellationToken);
            var resultUrl = string.Format(ApplicationConstant.ResultUrl, _appOptions.HostingUrl, token);

            switch (paymentMethod.PaymentMethodType)
            {
                case PaymentMethodEnum.Stripe:
                    var createStripeResponse = await HandleStripePaymentMethod
                    (
                        userCurrency,
                        (long)equivalentAmount,
                        transaction.Id,
                        resultUrl,
                        cancellationToken
                    );
                    if (createStripeResponse == null)
                    {
                        _logger.LogError(
                            $"{functionName} failed to create stripe payment method for transaction id: {transaction.Id}");
                        response.WithMessage(ErrorCode.PMM_ERR_002);
                        return response;
                    }

                    transaction.ProviderTxnId = createStripeResponse.Id;
                    response.WithData(createStripeResponse);
                    break;
                default:
                    _logger.LogError("{FunctionName:l} unsupported payment method type: {PaymentMethodPaymentMethodType:l}", functionName, paymentMethod.PaymentMethodType);
                    response.WithMessage(ErrorCode.PMM_ERR_001);
                    return response;
            }

            transaction.UserCurrencyCode = userCurrency;
            transaction.PaymentMethodId = paymentMethod.Id;
            transaction.Status = PaymentStatus.Processing;
            transaction.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.SaveAsync(cancellationToken);

            response
                .WithSuccess(true)
                .WithStatus(HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            ex.LogError(_logger, functionName);
            response.WithMessage(ErrorCode.EXH_ERR_001);
        }

        return response;
    }

    private async Task<StripeCreatePurchaseSessionResponse?> HandleStripePaymentMethod(string currency, long amount, Guid customId, string returnUrl, CancellationToken cancellationToken)
    {
        var createInvoiceResponse = await _stripeService.CreateCheckoutSessionAsync(
            new StripeCreatePurchaseSessionRequest
            {
                Currency = currency,
                Amount = amount * 100,
                CustomerId = customId,
                ReturnUrl = returnUrl,
            }, cancellationToken);

        return createInvoiceResponse;
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