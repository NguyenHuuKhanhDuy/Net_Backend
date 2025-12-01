using Backend_Net.Application.Features.BackgroundJobs.Commands.JobSendWebhook;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Backend_Net.Api.InternalControllers;

[ApiExplorerSettings(IgnoreApi = true)]
[Route("BackgroundJobInternal")]
[ApiController]
public class BackgroundJobInternalController : Controller
{
    private readonly IMediator _mediator;
    
    public BackgroundJobInternalController
    (
        IMediator mediator
    )
    {
        _mediator = mediator;
    }
    
    [HttpPost("JobSendWebhook")]
    public async Task<IActionResult> SendWebhook(CancellationToken cancellationToken)
    {
        await _mediator.Send(new JobSendWebhookCommand(), cancellationToken);
        return Ok();
    }
}