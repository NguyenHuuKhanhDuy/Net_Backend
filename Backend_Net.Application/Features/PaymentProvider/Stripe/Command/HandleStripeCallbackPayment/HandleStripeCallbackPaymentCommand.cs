using MediatR;
using Shared.MassTransit.IntegrationEvents;

namespace Backend_Net.Application.Features.PaymentProvider.Stripe.Command.HandleStripeCallbackPayment;

public class HandleStripeCallbackPaymentCommand : IRequest
{
    public HandleStripeCallbackPaymentEvent Payload { get; set; }

    public HandleStripeCallbackPaymentCommand(HandleStripeCallbackPaymentEvent payload)
    {
        Payload = payload;
    }
}