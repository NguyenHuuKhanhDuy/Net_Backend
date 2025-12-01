using Backend_Net.Domain.Enums;

namespace Backend_Net.Domain.Entities;

public class PaymentTransaction
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid TenantCredentialId { get; set; }
    public string? UserId { get; set; }
    public string OrderId { get; set; } = null!;
    public Guid? PaymentMethodId { get; set; }
    public string TenantCurrencyCode { get; set; } = null!;
    public string? UserCurrencyCode { get; set; }
    public decimal ExchangeRate { get; set; }
    public decimal Amount { get; set; }
    public decimal? Fee { get; set; }
    public PayerFeeType? PayerFeeType { get; set; }
    public decimal NetAmount { get; set; }
    public PaymentStatus Status { get; set; }
    public string? ProviderTxnId { get; set; }
    public string? ProviderRawReq { get; set; }
    public string? ProviderRawRes { get; set; }
    public string ReturnUrl { get; set; } = null!;
    public string CallbackUrl { get; set; } = null!;
    public string? CallbackData { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Tenant Tenant { get; set; } = null!;
    public TenantCredential TenantCredential { get; set; } = null!;
    public PaymentMethod? PaymentMethod { get; set; }
    public Currency TenantCurrency { get; set; } = null!;
    public Currency? UserCurrency { get; set; }
    public ICollection<PaymentTransactionStatusHistory> StatusHistories { get; set; } = new List<PaymentTransactionStatusHistory>();
    public ICollection<PaymentTransactionAudit> Audits { get; set; } = new List<PaymentTransactionAudit>();
    public ICollection<WebhookEvent> WebhookEvents { get; set; } = new List<WebhookEvent>();
}