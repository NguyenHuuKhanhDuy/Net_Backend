namespace Backend_Net.Domain.Entities;

public class TenantPaymentMethod
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid PaymentMethodId { get; set; }
    public bool IsEnabled { get; set; }
    public string? ConfigJson { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Tenant Tenant { get; set; } = null!;
    public PaymentMethod PaymentMethod { get; set; } = null!;
}