using Backend_Net.Domain.Entities;

namespace Backend_Net.Application.Common.Interfaces.Repositories;

public interface ITenantPaymentMethodRepository : IRepository<TenantPaymentMethod>
{
    Task<List<TenantPaymentMethod>> GetPaymentMethodsByTenantIdAsync(Guid tenantId, int countryId, Currency tenantCurrency, CancellationToken cancellationToken);
    Task<TenantPaymentMethod?> GetPaymentMethodByTenantIdAsync(Guid tenantId, Country country, Guid methodId, Currency tenantCurrency, CancellationToken cancellationToken);
}