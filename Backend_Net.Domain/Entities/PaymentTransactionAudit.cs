namespace Backend_Net.Domain.Entities;

public class PaymentTransactionAudit
{
    public Guid Id { get; set; }
    public Guid TransactionId { get; set; }
    public Guid TenantId { get; set; }

    public string FieldName { get; set; } = null!;
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }

    public DateTime ChangedAt { get; set; }
    public string? ChangedBy { get; set; }
    public string? Reason { get; set; }

    public PaymentTransaction Transaction { get; set; } = null!;
    public Tenant Tenant { get; set; } = null!;
}