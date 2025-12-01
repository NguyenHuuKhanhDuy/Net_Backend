using Backend_Net.Domain.Entities;

namespace Backend_Net.Application.Services.Rate;

public class RateService : IRateService
{
    private readonly Dictionary<string, decimal> _rates = new();
    private readonly object _lock = new();

    public void SetRates(IEnumerable<CurrencyRate> rates)
    {
        lock (_lock)
        {
            _rates.Clear();

            foreach (var r in rates)
            {
                var key = $"{r.BaseCurrencyCode}:{r.QuoteCurrencyCode}";
                _rates[key] = r.Rate;
            }
        }
    }

    public decimal? Convert(string baseCurrency, string quoteCurrency)
    {
        baseCurrency = baseCurrency.ToUpper();
        quoteCurrency = quoteCurrency.ToUpper();

        if (baseCurrency == quoteCurrency)
            return 1m;

        var directKey = $"{baseCurrency}:{quoteCurrency}";
        var reverseKey = $"{quoteCurrency}:{baseCurrency}";

        if (_rates.TryGetValue(directKey, out var direct))
            return direct;

        if (_rates.TryGetValue(reverseKey, out var reverse))
            return 1m / reverse;

        return null;
    }
}