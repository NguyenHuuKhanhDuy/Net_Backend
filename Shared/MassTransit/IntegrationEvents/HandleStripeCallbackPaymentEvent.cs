namespace Shared.MassTransit.IntegrationEvents;

public class HandleStripeCallbackPaymentEvent
{
    public Guid CorrelationId { get; set; }
    public string JsonData { get; set; }
    public string StripeSignature { get; set; }
}