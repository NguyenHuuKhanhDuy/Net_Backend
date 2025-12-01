using Backend_Net.Domain.Entities;

namespace Backend_Net.Application.Services.Rate;

public interface IRateService
{
    void SetRates(IEnumerable<CurrencyRate> rates);
    decimal? Convert(string baseCurrency, string quoteCurrency);
}