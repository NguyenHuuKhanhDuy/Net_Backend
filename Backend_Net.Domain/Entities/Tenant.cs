namespace Backend_Net.Domain.Entities;

public class Tenant
{
    public Guid Id { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public ICollection<TenantPaymentMethod> TenantPaymentMethods { get; set; } = new List<TenantPaymentMethod>();
    public ICollection<TenantPaymentMethodCurrency> TenantPaymentMethodCurrencies { get; set; } = new List<TenantPaymentMethodCurrency>();
    public ICollection<PaymentTransaction> PaymentTransactions { get; set; } = new List<PaymentTransaction>();
    public ICollection<PaymentTransactionAudit> PaymentTransactionAudits { get; set; } = new List<PaymentTransactionAudit>();
    public ICollection<WebhookEvent> WebhookEvents { get; set; } = new List<WebhookEvent>();
    public ICollection<TenantCredential> Credentials { get; set; } = new List<TenantCredential>();
}