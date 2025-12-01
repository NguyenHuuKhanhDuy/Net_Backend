using Microsoft.EntityFrameworkCore.Storage;

namespace Backend_Net.Application.Common.Interfaces.Repositories;

public interface IUnitOfWork : IAsyncDisposable, IDisposable
{
    ITenantRepository Tenant { get; }
    ICurrencyRepository Currency { get; }
    ICurrencyRateRepository CurrencyRate { get; }
    IPaymentMethodRepository PaymentMethod { get; }
    IPaymentMethodCurrencyRepository PaymentMethodCurrency { get; }
    ITenantPaymentMethodRepository TenantPaymentMethod { get; }
    ITenantPaymentMethodCurrencyRepository TenantPaymentMethodCurrency { get; }
    IPaymentTransactionRepository PaymentTransaction { get; }
    IPaymentTransactionStatusHistoryRepository PaymentTransactionStatusHistory { get; }
    IPaymentTransactionAuditRepository PaymentTransactionAudit { get; }
    IWebhookEventRepository WebhookEvent { get; }
    IWebhookDeliveryRepository WebhookDelivery { get; }
    IRequestLogRepository RequestLog { get; }
    ITenantCredentialRepository TenantCredential { get; }
    ICountryRepository Country { get; }

    Task SaveAsync(CancellationToken cancellationToken = default);
    Task<IDbContextTransaction> OpenTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitAsync(CancellationToken cancellationToken = default);
    Task RollbackAsync(CancellationToken cancellationToken = default);
}