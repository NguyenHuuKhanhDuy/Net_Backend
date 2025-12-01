using Backend_Net.Application.Common.Extensions;
using Backend_Net.Domain.Enums;
using FluentValidation;

namespace Backend_Net.Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderValidator()
    {
        RuleFor(command => command)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty()
            .WithErrorCode(ErrorCode.DAT_ERR_002.Code())
            .WithMessage(ErrorCode.DAT_ERR_002.Localize());
        
        RuleFor(command => command.ApiKey)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty()
            .WithErrorCode(ErrorCode.DAT_ERR_001.Code())
            .WithMessage(ErrorCode.DAT_ERR_001.Localize());
        
        RuleFor(command => command.Signature)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty()
            .WithErrorCode(ErrorCode.DAT_ERR_001.Code())
            .WithMessage(ErrorCode.DAT_ERR_001.Localize());
        
        RuleFor(command => command.Payload.Amount)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty()
            .WithErrorCode(ErrorCode.DAT_ERR_001.Code())
            .WithMessage(ErrorCode.DAT_ERR_001.Localize());
            
        RuleFor(command => command.Payload.Currency)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty()
            .WithErrorCode(ErrorCode.DAT_ERR_001.Code())
            .WithMessage(ErrorCode.DAT_ERR_001.Localize());
        
        RuleFor(command => command.Payload.ReturnUrl)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty()
            .WithErrorCode(ErrorCode.DAT_ERR_001.Code())
            .WithMessage(ErrorCode.DAT_ERR_001.Localize());
        
        RuleFor(command => command.Payload.CallbackUrl)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty()
            .WithErrorCode(ErrorCode.DAT_ERR_001.Code())
            .WithMessage(ErrorCode.DAT_ERR_001.Localize());
        
        RuleFor(command => command.Payload.ReferenceId)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty()
            .WithErrorCode(ErrorCode.DAT_ERR_001.Code())
            .WithMessage(ErrorCode.DAT_ERR_001.Localize());
    }
}