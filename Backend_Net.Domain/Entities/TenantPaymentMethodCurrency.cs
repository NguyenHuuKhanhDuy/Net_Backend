namespace Backend_Net.Domain.Entities;

public class TenantPaymentMethodCurrency
{
    public Guid TenantId { get; set; }
    public Guid PaymentMethodId { get; set; }
    public string CurrencyCode { get; set; } = null!;

    public bool IsEnabled { get; set; }
    public decimal? MinAmount { get; set; }
    public decimal? MaxAmount { get; set; }

    public string? FeeType { get; set; }
    public decimal? FeeValue { get; set; }

    public Tenant Tenant { get; set; } = null!;
    public PaymentMethod PaymentMethod { get; set; } = null!;
    public Currency Currency { get; set; } = null!;
    public PaymentMethodCurrency GlobalPaymentMethodCurrency { get; set; } = null!;
}