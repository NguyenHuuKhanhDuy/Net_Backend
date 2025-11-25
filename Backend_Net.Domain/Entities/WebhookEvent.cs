namespace Backend_Net.Domain.Entities;

public class WebhookEvent
{
    public Guid Id { get; set; }

    public string EventType { get; set; } = null!;
    public Guid TenantId { get; set; }

    public Guid? TransactionId { get; set; }

    public string CallbackUrl { get; set; } = null!;
    public string? CallbackData { get; set; }

    public string Payload { get; set; } = null!;
    public DateTime CreatedAt { get; set; }

    public Tenant Tenant { get; set; } = null!;
    public PaymentTransaction? Transaction { get; set; }
    public ICollection<WebhookDelivery> Deliveries { get; set; } = new List<WebhookDelivery>();
}