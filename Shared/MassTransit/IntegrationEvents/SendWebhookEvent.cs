namespace Shared.MassTransit.IntegrationEvents;

public class SendWebhookEvent
{
    public List<Guid> WebhookDeliveryIds { get; set; }
}