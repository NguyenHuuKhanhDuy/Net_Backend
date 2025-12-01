using MediatR;

namespace Backend_Net.Application.Features.PaymentProvider.Stripe.Command.CreatePaymentSession;

public class CreatePaymentSessionCommand : IRequest<CreatePaymentSessionResponse>
{
    public CreatePaymentSessionRequest Payload { get; set; }

    public CreatePaymentSessionCommand(CreatePaymentSessionRequest payload)
    {
        Payload = payload;
    }
}