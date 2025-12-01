using Backend_Net.Application.Services.Webhook;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.Extensions;
using Shared.Helpers;

namespace Backend_Net.Application.Features.WebHooks.Commands.SendWebhook;

public class SendWebhookHandler : IRequestHandler<SendWebhookCommand>
{
    private readonly ILogger<SendWebhookHandler> _logger;
    private readonly IWebhookService _webhookService;

    public SendWebhookHandler(ILogger<SendWebhookHandler> logger, IWebhookService webhookService)
    {
        _logger = logger;
        _webhookService = webhookService;
    }

    #region Implementation of IRequestHandler<in SendWebhookCommand, SendWebhookResponse>

    public async Task Handle(SendWebhookCommand request, CancellationToken cancellationToken)
    {
        var payload = request.Payload;
        var functionName = $"{nameof(SendWebhookHandler)} =>";
        _logger.LogInformation("{FunctionName:l} Payload: {Payload:l}", functionName, JsonHelper.Serialize(payload));

        try
        {
            await _webhookService.ExecuteWebhookBatchAsync(payload.WebhookDeliveryIds, cancellationToken);
        }
        catch (Exception e)
        {
            e.LogError(_logger, functionName);
        }
    }

    #endregion
}