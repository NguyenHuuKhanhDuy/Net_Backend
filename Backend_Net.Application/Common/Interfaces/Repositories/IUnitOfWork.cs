using Microsoft.EntityFrameworkCore.Storage;

namespace Backend_Net.Application.Common.Interfaces.Repositories;

public interface IUnitOfWork
{
    ITenantRepository Tenant { get; }
    ICurrencyRepository Currency { get; }
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
    
    Task SaveAsync();
    Task<IDbContextTransaction> OpenTransactionAsync();
    Task CommitAsync();
    Task RollbackAsync();
}