using Backend_Net.Application.Common.Interfaces.MassTransit;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Backend_Net.Infrastructure.MassTransit;

public class SendEndpointCustomProvider : IMessageSender
{
    private readonly IBusControl _busControl;
    private readonly ILogger<SendEndpointCustomProvider> _logger;

    public SendEndpointCustomProvider
    (
        IBusControl busControl, 
        ILogger<SendEndpointCustomProvider> logger
    )
    {
        _busControl = busControl;
        _logger = logger;
    }
    
    public async Task SendMessage<T>(object eventModel, CancellationToken cancellationToken) where T : class
    {
        const string funcName = $"{nameof(SendEndpointCustomProvider)} {nameof(SendMessage)} =>";
        try
        {
            _logger.LogInformation($"{funcName} is called ...");
            var kebabFormatter =  new KebabCaseEndpointNameFormatter(false);
            var queueName = kebabFormatter.SanitizeName(typeof(T).Name);
        
            if (!string.IsNullOrWhiteSpace(queueName))
            {
                _logger.LogInformation($"{funcName} QueueName: {queueName}");
                var sendEndpoint = await GetSendEndpoint(new Uri($"queue:{queueName}"));
                await sendEndpoint.Send<T>(eventModel, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(SendEndpointCustomProvider)} {nameof(SendMessage)} => {ex.Message}");
        }
    }

    public Task<ISendEndpoint> GetSendEndpoint(Uri address)
    {
        return _busControl.GetSendEndpoint(address);
    }

    public ConnectHandle ConnectSendObserver(ISendObserver observer)
    {
        return _busControl.ConnectSendObserver(observer);
    }
}