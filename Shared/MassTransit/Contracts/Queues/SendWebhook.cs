using MassTransit;
using Shared.MassTransit.IntegrationEvents;

namespace Shared.MassTransit.Contracts.Queues;

[ConfigureConsumeTopology(false)]
public interface SendWebhook : CorrelatedBy<Guid> 
{
    public SendWebhookEvent Content { get; }
}