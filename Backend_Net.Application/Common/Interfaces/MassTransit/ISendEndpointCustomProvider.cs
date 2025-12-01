namespace Backend_Net.Application.Common.Interfaces.MassTransit;

public interface IMessageSender
{
    Task SendMessage<T>(object eventModel, CancellationToken cancellationToken) where T : class;
}