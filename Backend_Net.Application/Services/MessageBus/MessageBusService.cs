using Backend_Net.Application.Features.PaymentProvider.Stripe.Command.HandleStripeCallbackPayment;
using Backend_Net.Application.Features.WebHooks.Commands.SendWebhook;
using MediatR;
using Shared.MassTransit.IntegrationEvents;

namespace Backend_Net.Application.Services.MessageBus;

public class MessageBusService : IMessageBusService
{
    private readonly IMediator _mediator;

    public MessageBusService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task SendWebhookAsync(SendWebhookEvent message, CancellationToken cancellationToken)
    {
        await _mediator.Send(new SendWebhookCommand(new SendWebhookRequest
            { WebhookDeliveryIds = message.WebhookDeliveryIds }), cancellationToken);
    }

    public async Task HandleStripeCallbackPaymentAsync(HandleStripeCallbackPaymentEvent message, CancellationToken cancellationToken)
    {
        await _mediator.Send(new HandleStripeCallbackPaymentCommand(message), cancellationToken);
    }
}