using Shared.MassTransit.IntegrationEvents;

namespace Backend_Net.Application.Services.MessageBus;

public interface IMessageBusService
{
    Task SendWebhookAsync(SendWebhookEvent message, CancellationToken cancellationToken);
    Task HandleStripeCallbackPaymentAsync(HandleStripeCallbackPaymentEvent message, CancellationToken cancellationToken);
}