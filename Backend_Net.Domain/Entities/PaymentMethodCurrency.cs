namespace Backend_Net.Domain.Entities;

public class PaymentMethodCurrency
{
    public Guid PaymentMethodId { get; set; }
    public string CurrencyCode { get; set; } = null!;
    public bool IsActive { get; set; }
    public decimal? MinAmount { get; set; }
    public decimal? MaxAmount { get; set; }

    public PaymentMethod PaymentMethod { get; set; } = null!;
    public Currency Currency { get; set; } = null!;
    public ICollection<TenantPaymentMethodCurrency> TenantPaymentMethodCurrencies { get; set; } = new List<TenantPaymentMethodCurrency>();
}