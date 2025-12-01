using Backend_Net.Domain.Models.Stripe;

namespace Backend_Net.Application.Common.Interfaces.PaymentProvider;

public interface IStripeService
{
    Task<StripeCreatePurchaseSessionResponse?> CreateCheckoutSessionAsync(StripeCreatePurchaseSessionRequest request, CancellationToken cancellationToken = default);
    Task<bool> IsValidWebhook(HandleStripeCallbackPaymentRequest request, CancellationToken cancellationToken = default);
}