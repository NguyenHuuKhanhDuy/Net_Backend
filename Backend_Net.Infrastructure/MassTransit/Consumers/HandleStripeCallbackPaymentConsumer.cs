using Backend_Net.Application.Services.MessageBus;
using MassTransit;
using Microsoft.Extensions.Logging;
using Shared.Extensions;
using Shared.MassTransit.Contracts.Queues;

namespace Backend_Net.Infrastructure.MassTransit.Consumers;

public class HandleStripeCallbackPaymentConsumer : IConsumer<HandleStripeCallbackPayment>
{
    private readonly ILogger<HandleStripeCallbackPaymentConsumer> _logger;
    private readonly IMessageBusService _messageBusService;
    
    public HandleStripeCallbackPaymentConsumer
    (
        ILogger<HandleStripeCallbackPaymentConsumer> logger,
        IMessageBusService messageBusService
    )
    {
        _logger = logger;
        _messageBusService = messageBusService;
    }
    
    public async Task Consume(ConsumeContext<HandleStripeCallbackPayment> context)
    {
        var message = context.Message;
        var functionName = $"{nameof(HandleStripeCallbackPaymentConsumer)} CorrelationId = {message.CorrelationId} =>";
        _logger.LogInformation($"{functionName}");

        try
        {
            message.Content.CorrelationId = message.CorrelationId;
            await _messageBusService.HandleStripeCallbackPaymentAsync(message.Content, CancellationToken.None);
        }
        catch (Exception ex)
        {
            ex.LogError(_logger, functionName);
        }
    }
}

class HandleStripeCallbackPaymentConsumerDefinition : ConsumerDefinition<HandleStripeCallbackPaymentConsumer>
{
    public HandleStripeCallbackPaymentConsumerDefinition()
    {
    }

    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<HandleStripeCallbackPaymentConsumer> consumerConfigurator)
    {
        endpointConfigurator.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(30)));
    }
}