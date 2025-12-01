using Backend_Net.Application.Services.MessageBus;
using MassTransit;
using Microsoft.Extensions.Logging;
using Shared.Extensions;
using Shared.MassTransit.Contracts.Queues;

namespace Backend_Net.Infrastructure.MassTransit.Consumers;

public class SendWebhookConsumer : IConsumer<SendWebhook>
{
    private readonly ILogger<SendWebhookConsumer> _logger;
    private readonly IMessageBusService _messageBusService;
    
    public SendWebhookConsumer(ILogger<SendWebhookConsumer> logger, IMessageBusService messageBusService)
    {
        _logger = logger;
        _messageBusService = messageBusService;
    }
    
    public async Task Consume(ConsumeContext<SendWebhook> context)
    {
        var message = context.Message;
        var functionName = $"{nameof(SendWebhookConsumer)} CorrelationId = {message.CorrelationId} =>";
        _logger.LogInformation($"{functionName}");
        
        try
        {
            await _messageBusService.SendWebhookAsync(message.Content, context.CancellationToken);
        }     
        catch (Exception ex)
        {
            ex.LogError(_logger, functionName);
        }
    }
}

class SendWebhookDefinition : ConsumerDefinition<SendWebhookConsumer>
{
    public SendWebhookDefinition()
    {
    }

    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<SendWebhookConsumer> consumerConfigurator)
    {
        endpointConfigurator.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(30)));
    }
}