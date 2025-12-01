using FluentValidation;

namespace Backend_Net.Application.Features.Orders.Commands.CancelOrder;

public class CancelOrderValidator : AbstractValidator<CancelOrderCommand>
{
    public CancelOrderValidator()
    {
        // Add validation rules here if needed
    }
}