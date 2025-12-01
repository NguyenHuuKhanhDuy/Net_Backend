namespace Backend_Net.Domain.Entities;

public class CurrencyRate
{
    public Guid Id { get; set; }

    public string BaseCurrencyCode { get; set; } = default!;
    public string QuoteCurrencyCode { get; set; } = default!;

    public decimal Rate { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Currency? BaseCurrency { get; set; }
    public Currency? QuoteCurrency { get; set; }
}