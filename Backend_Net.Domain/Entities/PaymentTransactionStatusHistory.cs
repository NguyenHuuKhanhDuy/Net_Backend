using Backend_Net.Domain.Enums;

namespace Backend_Net.Domain.Entities;

public class PaymentTransactionStatusHistory
{
    public Guid Id { get; set; }
    public Guid TransactionId { get; set; }

    public PaymentStatus? FromStatus { get; set; }
    public PaymentStatus ToStatus { get; set; }

    public DateTime ChangedAt { get; set; }
    public string? ChangedBy { get; set; }
    public string? Reason { get; set; }

    public PaymentTransaction Transaction { get; set; } = null!;
}