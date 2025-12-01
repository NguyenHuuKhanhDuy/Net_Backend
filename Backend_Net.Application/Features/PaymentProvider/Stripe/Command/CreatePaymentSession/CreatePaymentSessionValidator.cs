using FluentValidation;

namespace Backend_Net.Application.Features.PaymentProvider.Stripe.Command.CreatePaymentSession;

public class CreatePaymentSessionValidator : AbstractValidator<CreatePaymentSessionCommand>
{
    public CreatePaymentSessionValidator()
    {
        // Add validation rules here if needed
    }
}