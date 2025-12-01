using Backend_Net.Domain.Enums;

namespace Backend_Net.Domain.Entities;

public class Currency
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public int MinorUnit { get; set; }
    public bool IsActive { get; set; }
    public bool IsGlobal { get; set; }
    public CurrencyType Type { get; set; }
    public ICollection<PaymentMethodCurrency> PaymentMethodCurrencies { get; set; } = new List<PaymentMethodCurrency>();
    public ICollection<TenantPaymentMethodCurrency> TenantPaymentMethodCurrencies { get; set; } = new List<TenantPaymentMethodCurrency>();
    public ICollection<PaymentTransaction> TenantPaymentTransactions { get; set; } = new List<PaymentTransaction>();
    public ICollection<PaymentTransaction> UserPaymentTransactions { get; set; } = new List<PaymentTransaction>();
    public ICollection<CurrencyRate> BaseCurrencyRates { get; set; } = new List<CurrencyRate>();
    public ICollection<CurrencyRate> QuoteCurrencyRates { get; set; } = new List<CurrencyRate>();
    public ICollection<Country> Countries { get; set; } = new List<Country>();
}