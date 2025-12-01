namespace Backend_Net.Application.Models.Dtos;

public class WebhookQueueDto
{
    public Guid TenantId { get; set; }
    public Guid TransactionId { get; set; }
    public string CallbackUrl { get; set; }
    public object Payload { get; set; }
}