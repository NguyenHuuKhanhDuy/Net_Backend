using Backend_Net.Domain.Enums;

namespace Backend_Net.Domain.Entities;

public class PaymentMethod
{
    public Guid Id { get; set; }
    public string Code { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public string? BackgroundUrl { get; set; }
    public PaymentMethodEnum PaymentMethodType { get; set; }
    public PaymentMethodType Type { get; set; }
    public PaymentMethodRuleType RuleType { get; set; }
    public int[] RuleData { get; set; } = [];
    public bool IsActive { get; set; }
    public decimal Fee { get; set; }
    public FeeType FeeType { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ICollection<PaymentMethodCurrency> PaymentMethodCurrencies { get; set; } = new List<PaymentMethodCurrency>();
    public ICollection<TenantPaymentMethod> TenantPaymentMethods { get; set; } = new List<TenantPaymentMethod>();
    public ICollection<TenantPaymentMethodCurrency> TenantPaymentMethodCurrencies { get; set; } = new List<TenantPaymentMethodCurrency>();
    public ICollection<PaymentTransaction> PaymentTransactions { get; set; } = new List<PaymentTransaction>();
}