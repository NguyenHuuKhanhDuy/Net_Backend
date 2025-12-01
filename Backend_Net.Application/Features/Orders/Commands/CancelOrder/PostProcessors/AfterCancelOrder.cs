using Backend_Net.Application.Common.Extensions;
using Backend_Net.Application.Services.Webhook;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using Shared.Extensions;

namespace Backend_Net.Application.Features.Orders.Commands.CancelOrder.PostProcessors;

public class AfterCancelOrder : IRequestPostProcessor<CancelOrderCommand, CancelOrderResponse>
{
    private readonly ILogger<AfterCancelOrder> _logger;
    private readonly IWebhookService _webhookService;
    
    public AfterCancelOrder(ILogger<AfterCancelOrder> logger, IWebhookService webhookService)
    {
        _logger = logger;
        _webhookService = webhookService;
    }
    
    public async Task Process(CancelOrderCommand request, CancelOrderResponse response, CancellationToken cancellationToken)
    {
        var funcName = $"{nameof(AfterCancelOrder)} =>";
        _logger.LogInformation(funcName);

        try
        {
            if (response.Success)
            {
                await _webhookService.QueueWebhookAsync(response.PaymentTransactionId, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            ex.LogError(_logger, funcName);
        }
    }
}