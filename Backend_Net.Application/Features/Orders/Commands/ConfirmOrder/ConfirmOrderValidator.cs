using FluentValidation;

namespace Backend_Net.Application.Features.Orders.Commands.ConfirmOrder;

public class ConfirmOrderValidator : AbstractValidator<ConfirmOrderCommand>
{
    public ConfirmOrderValidator()
    {
        // Add validation rules here if needed
    }
}