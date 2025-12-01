namespace Backend_Net.Domain.Models.Stripe;

public class HandleStripeCallbackPaymentRequest
{
    public string StripeSignature { get; set; }
    public string JsonData { get; set; }
}