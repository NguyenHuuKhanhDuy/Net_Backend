using FluentValidation;

namespace Backend_Net.Application.Features.WebHooks.Commands.SendWebhook;

public class SendWebhookValidator : AbstractValidator<SendWebhookCommand>
{
    public SendWebhookValidator()
    {
        // Add validation rules here if needed
    }
}