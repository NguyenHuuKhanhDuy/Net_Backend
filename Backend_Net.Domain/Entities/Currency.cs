namespace Backend_Net.Domain.Entities;

public class Currency
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public int MinorUnit { get; set; }
    public bool IsActive { get; set; }

    public ICollection<PaymentMethodCurrency> PaymentMethodCurrencies { get; set; } = new List<PaymentMethodCurrency>();
    public ICollection<TenantPaymentMethodCurrency> TenantPaymentMethodCurrencies { get; set; } = new List<TenantPaymentMethodCurrency>();
    public ICollection<PaymentTransaction> PaymentTransactions { get; set; } = new List<PaymentTransaction>();
}