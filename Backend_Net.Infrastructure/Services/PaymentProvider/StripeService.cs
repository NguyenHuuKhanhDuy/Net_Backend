using Backend_Net.Application.Common.Interfaces.PaymentProvider;
using Backend_Net.Application.Options;
using Backend_Net.Domain.Models.Stripe;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Extensions;
using Shared.Helpers;
using Stripe;
using Stripe.Checkout;

namespace Backend_Net.Infrastructure.Services.PaymentProvider;

public class StripeService : IStripeService
{
    private readonly ILogger<StripeService> _logger;
    private readonly PaymentSettingsOptions _paymentSettings;

    public StripeService
    (
        ILogger<StripeService> logger,
        IOptions<PaymentSettingsOptions> paymentSettings
    )
    {
        _paymentSettings = paymentSettings.Value;
        _logger = logger;
    }
    
    public async Task<StripeCreatePurchaseSessionResponse?> CreateCheckoutSessionAsync(StripeCreatePurchaseSessionRequest request, CancellationToken cancellationToken = default)
    {
         var functionName = $"{nameof(StripeService)} {nameof(CreateCheckoutSessionAsync)} =>";
        _logger.LogInformation("{functionName:l} Payload = {payload:l}", functionName, JsonHelper.Serialize(request));
        var response = new StripeCreatePurchaseSessionResponse();
        
        try
        {
            var stripeSettings = _paymentSettings.Stripe;
            StripeConfiguration.ApiKey = stripeSettings.ApiKey;

            var sessionLineItemOption = new SessionLineItemOptions
            {
                Quantity = 1,
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = request.Amount,
                    Currency = request.Currency.ToLower(),
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = "WePayment Purchase",
                        Description = "Purchase via WePayment"
                    }
                }
            };

            var service = new SessionService();

           var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>
                {
                    sessionLineItemOption
                },
                Mode = "payment",
                Metadata = new Dictionary<string, string>
                {
                    { "transactionId", request.CustomerId.ToString() }
                },
                PaymentIntentData = new SessionPaymentIntentDataOptions
                {
                    SetupFutureUsage = "off_session"
                },
                SuccessUrl = request.ReturnUrl,
                CancelUrl = request.ReturnUrl
            };

            var requestOptions = new RequestOptions
            {
                IdempotencyKey = Guid.NewGuid().ToString()
            };
            var session = await service.CreateAsync(options, requestOptions, cancellationToken);
            if (session == null) return null;

            response.Id = session.Id;
            response.PaymentUrl = session.Url;
            return response;
        }
        catch (Exception e)
        {
            _logger.LogInformation("{functionName:l} Exception = {exception:l}", functionName, e);
            return null;
        }
    }

    public async Task<bool> IsValidWebhook(HandleStripeCallbackPaymentRequest request, CancellationToken cancellationToken = default)
    {
        var functionName = $"{nameof(StripeService)} {nameof(IsValidWebhook)} =>";
        _logger.LogInformation("{functionName:l} Payload = {payload:l}", functionName, JsonHelper.Serialize(request));

        try
        {
            var stripeSettings = _paymentSettings.Stripe;
            StripeConfiguration.ApiKey = stripeSettings.ApiKey;
            
            var stripeEvent = EventUtility.ConstructEvent
            (
                request.JsonData,
                request.StripeSignature,
                stripeSettings.WebhookSecret,
                throwOnApiVersionMismatch: false
            );
            if (stripeEvent is null)
            {
                _logger.LogWarning("{functionName:l} Cannot parse stripe event", functionName);
                return false;
            }

            return true;
        }
        catch (Exception e)
        {
            e.LogError(_logger, functionName);
            return false;
        }
    }
}