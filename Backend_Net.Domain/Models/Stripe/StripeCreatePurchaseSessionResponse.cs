namespace Backend_Net.Domain.Models.Stripe;

public class StripeCreatePurchaseSessionResponse
{
    public string Id { get; set; }
    public string PaymentUrl { get; set; }
}