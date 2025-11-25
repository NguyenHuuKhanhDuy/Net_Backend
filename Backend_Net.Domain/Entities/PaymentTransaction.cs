using Backend_Net.Domain.Enums;

namespace Backend_Net.Domain.Entities;

public class PaymentTransaction
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string? UserId { get; set; }
    public string? OrderId { get; set; }
    public Guid? PaymentMethodId { get; set; }
    public string? CurrencyCode { get; set; }
    public decimal Amount { get; set; }
    public decimal? FeeAmount { get; set; }
    public decimal? NetAmount { get; set; }
    public PaymentStatus Status { get; set; }
    public string? ProviderTxnId { get; set; }
    public string? ProviderRawReq { get; set; }
    public string? ProviderRawRes { get; set; }
    public string? ReturnUrl { get; set; }
    public string? CallbackUrl { get; set; }
    public string? CallbackData { get; set; }

    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Tenant Tenant { get; set; } = null!;
    public PaymentMethod? PaymentMethod { get; set; }
    public Currency? Currency { get; set; }
    public ICollection<PaymentTransactionStatusHistory> StatusHistories { get; set; } = new List<PaymentTransactionStatusHistory>();
    public ICollection<PaymentTransactionAudit> Audits { get; set; } = new List<PaymentTransactionAudit>();
    public ICollection<WebhookEvent> WebhookEvents { get; set; } = new List<WebhookEvent>();
}