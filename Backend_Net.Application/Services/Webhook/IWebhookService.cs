using Backend_Net.Application.Models.Dtos;

namespace Backend_Net.Application.Services.Webhook;

public interface IWebhookService
{
    Task<Guid> QueueWebhookAsync(Guid transactionId, CancellationToken cancellationToken);

    Task ExecuteWebhookBatchAsync(List<Guid> webhookDeliveryIds, CancellationToken cancellationToken);
}