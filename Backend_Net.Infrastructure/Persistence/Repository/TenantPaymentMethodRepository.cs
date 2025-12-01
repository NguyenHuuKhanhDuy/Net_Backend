using Backend_Net.Application.Common.Interfaces.Repositories;
using Backend_Net.Application.Constants;
using Backend_Net.Domain.Entities;
using Backend_Net.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Backend_Net.Infrastructure.Persistence.Repository;

public class TenantPaymentMethodRepository(AppDbContext context, ILogger logger)
    : Repository<TenantPaymentMethod>(context, logger), ITenantPaymentMethodRepository
{
    public async Task<List<TenantPaymentMethod>> GetPaymentMethodsByTenantIdAsync(Guid tenantId, int countryId, Currency tenantCurrency, CancellationToken cancellationToken)
    {
        var country = await context.Countries
            .Include(x => x.BaseCurrency)
            .FirstOrDefaultAsync(x => x.Id == countryId, cancellationToken);
        if (country is null)
            return [];
        
        var tenantMethods = await context.TenantPaymentMethods
            .Where(x => x.TenantId == tenantId)
            .Include(x => x.PaymentMethod)
            .ThenInclude(pm => pm.PaymentMethodCurrencies)
            .ToListAsync(cancellationToken);
        var availableForCountry = tenantMethods
            .Where(x => IsMethodAvailable(
                country.Id,
                x.PaymentMethod.RuleType,
                x.PaymentMethod.RuleData
            ))
            .ToList();

        if (tenantCurrency.Code == ApplicationConstant.Currencies.USD)
        {
            return availableForCountry
                .Where(pm =>
                    pm.PaymentMethod.Type == PaymentMethodType.Global
                    || pm.PaymentMethod.PaymentMethodCurrencies.Any(cc => cc.CurrencyCode == country.CurrencyCode)
                )
                .ToList();
        }

        if (tenantCurrency.Type == CurrencyType.Crypto)
        {
            return availableForCountry
                .Where(pm => 
                    pm.PaymentMethod.PaymentMethodCurrencies
                        .Any(cc => cc.CurrencyCode == tenantCurrency.Code)
                )
                .ToList();
        }

        if (country.CurrencyCode != tenantCurrency.Code)
        {
            return availableForCountry
                .Where(pm => pm.PaymentMethod.Type == PaymentMethodType.Global)
                .ToList();
        }
        
        return availableForCountry
            .Where(pm =>
                pm.PaymentMethod.Type == PaymentMethodType.Global
                || pm.PaymentMethod.PaymentMethodCurrencies.Any(cc => cc.CurrencyCode == country.CurrencyCode)
            )
            .ToList();
    }

    public async Task<TenantPaymentMethod?> GetPaymentMethodByTenantIdAsync(Guid tenantId, Country country, Guid methodId, Currency tenantCurrency, CancellationToken cancellationToken)
    {
        var tenantMethod = await context.TenantPaymentMethods
            .Where(x => x.TenantId == tenantId && x.Id == methodId)
            .Include(x => x.PaymentMethod)
            .ThenInclude(pm => pm.PaymentMethodCurrencies)
            .FirstOrDefaultAsync(cancellationToken);

        if (tenantMethod is null)
            return null;

        var pm = tenantMethod.PaymentMethod;
        var availableForCountry = IsMethodAvailable(
            country.Id,
            pm.RuleType,
            pm.RuleData
        );

        if (!availableForCountry)
            return null;

        var a = pm.Type == PaymentMethodType.Global
                           || pm.PaymentMethodCurrencies.Any(c => c.CurrencyCode == country.CurrencyCode);
        if (tenantCurrency.Code == ApplicationConstant.Currencies.USD)
            return a ? tenantMethod : null;

        if (tenantCurrency.Type == CurrencyType.Crypto)
        {
            var supportCrypto = pm.PaymentMethodCurrencies
                .Any(c => c.CurrencyCode == tenantCurrency.Code);

            return supportCrypto ? tenantMethod : null;
        }
        
        if (country.CurrencyCode != tenantCurrency.Code)
        {
            return pm.Type == PaymentMethodType.Global ? tenantMethod : null;
        }
        
        var supportLocal = pm.Type == PaymentMethodType.Global
                           || pm.PaymentMethodCurrencies.Any(c => c.CurrencyCode == tenantCurrency.Code);
        return supportLocal ? tenantMethod : null;
    }

    private bool IsMethodAvailable(int countryId, PaymentMethodRuleType ruleType, int[] ruleData)
    {
        return ruleType switch
        {
            PaymentMethodRuleType.All => true,
            PaymentMethodRuleType.None => false,
            PaymentMethodRuleType.Only => ruleData.Contains(countryId),
            PaymentMethodRuleType.Except => !ruleData.Contains(countryId),
            _ => false
        };
    }
}