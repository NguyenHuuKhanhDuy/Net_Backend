using FluentValidation;

namespace Backend_Net.Application.Features.BackgroundJobs.Commands.JobSendWebhook;

public class JobSendWebhookValidator : AbstractValidator<JobSendWebhookCommand>
{
    public JobSendWebhookValidator()
    {
        // Add validation rules here if needed
    }
}