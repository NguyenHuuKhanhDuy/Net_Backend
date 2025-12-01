using System.Net;
using Backend_Net.Application.Common.Interfaces.MassTransit;
using Microsoft.AspNetCore.Mvc;
using Shared.Extensions;
using Shared.MassTransit.Contracts.Queues;
using Shared.MassTransit.IntegrationEvents;

namespace Backend_Net.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WebhookController : Controller
{
    private readonly IMessageSender _messageSender;
    private readonly ILogger<WebhookController> _logger;
    public WebhookController(IMessageSender messageSender, ILogger<WebhookController> logger)
    {
        _messageSender = messageSender;
        _logger = logger;
    }
    
    [HttpPost("Stripe/CallbackPayment")]
    public async Task<IActionResult> StripeCallbackPayment(CancellationToken cancellationToken)
    {
        var correlationId = Guid.NewGuid();
        var functionName = $"{nameof(WebhookController)} {nameof(StripeCallbackPayment)} CorrelationId = {correlationId} =>";
        _logger.LogInformation($"{functionName}");
        
        try
        {
            using var reader = new StreamReader(HttpContext.Request.Body);
            var jsonData = await reader.ReadToEndAsync(cancellationToken);
            var eventMessage = new HandleStripeCallbackPaymentEvent
            {
                CorrelationId = correlationId,
                JsonData = jsonData,
                StripeSignature = Request.Headers["Stripe-Signature"],
            };

            await _messageSender.SendMessage<HandleStripeCallbackPayment>(new
            {
                Content = eventMessage,
                CorrelationId = correlationId
            }, cancellationToken);
            
            return Ok();
        }
        catch(Exception ex)
        {
            ex.LogError(_logger, functionName);
            return StatusCode((int)HttpStatusCode.InternalServerError);
        }
    }
}