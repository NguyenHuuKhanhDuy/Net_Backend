namespace Backend_Net.Domain.Entities;

public class Country
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string FlagUrl { get; set; } = null!;
    public string DialCode { get; set; } = null!;
    public string Alpha2Code { get; set; } = null!;
    public string Alpha3Code { get; set; } = null!;
    public string CurrencyCode { get; set; } = null!;
    public Currency? BaseCurrency { get; set; }
}