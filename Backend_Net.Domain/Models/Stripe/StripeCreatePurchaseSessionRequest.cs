namespace Backend_Net.Domain.Models.Stripe;

public class StripeCreatePurchaseSessionRequest
{
    public Guid CustomerId { get; set; }
    public long Amount { get; set; }
    public string Currency { get; set; }
    public string ReturnUrl { get; set; }
}