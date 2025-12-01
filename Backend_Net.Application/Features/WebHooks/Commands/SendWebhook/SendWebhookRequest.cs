namespace Backend_Net.Application.Features.WebHooks.Commands.SendWebhook;

public class SendWebhookRequest
{
    public List<Guid> WebhookDeliveryIds { get; set; }
}