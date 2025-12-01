using MediatR;

namespace Backend_Net.Application.Features.WebHooks.Commands.SendWebhook;

public class SendWebhookCommand : IRequest
{
    public SendWebhookRequest Payload { get; set; }

    public SendWebhookCommand(SendWebhookRequest payload)
    {
        Payload = payload;
    }
}