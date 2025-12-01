using Backend_Net.Application.Common.Extensions;
using Backend_Net.Domain.Enums;
using FluentValidation;

namespace Backend_Net.Application.Features.PaymentMethods.Queries.GetAvailablePaymentMethod;

public class GetAvailablePaymentMethodValidator : AbstractValidator<GetAvailablePaymentMethodQuery>
{
    public GetAvailablePaymentMethodValidator()
    {
        RuleFor(query => query)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty()
            .WithErrorCode(ErrorCode.DAT_ERR_002.Code())
            .WithMessage(ErrorCode.DAT_ERR_002.Localize());
        
        RuleFor(command => command.Payload)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty()
            .WithErrorCode(ErrorCode.DAT_ERR_002.Code())
            .WithMessage(ErrorCode.DAT_ERR_002.Localize());
        
        RuleFor(command => command.Payload.Token)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty()
            .WithErrorCode(ErrorCode.DAT_ERR_001.Code())
            .WithMessage(ErrorCode.DAT_ERR_001.Localize());
        
        RuleFor(command => command.Payload.CountryId)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty()
            .WithErrorCode(ErrorCode.DAT_ERR_001.Code())
            .WithMessage(ErrorCode.DAT_ERR_001.Localize());
    }
}