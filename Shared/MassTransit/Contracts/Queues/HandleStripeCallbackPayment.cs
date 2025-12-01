using MassTransit;
using Shared.MassTransit.IntegrationEvents;

namespace Shared.MassTransit.Contracts.Queues;

[ConfigureConsumeTopology(false)]
public interface HandleStripeCallbackPayment : CorrelatedBy<Guid>
{
    HandleStripeCallbackPaymentEvent Content {get; }
}