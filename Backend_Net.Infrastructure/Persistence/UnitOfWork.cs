using Backend_Net.Application.Common.Interfaces.Repositories;
using Backend_Net.Infrastructure.Persistence.Repository;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace Backend_Net.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly AppDbContext _context;
    private IDbContextTransaction _transaction;
    private readonly ILogger _logger;

    public UnitOfWork(AppDbContext context, ILoggerFactory logger)
    {
        _context = context;
        _logger = logger.CreateLogger("logs");

        Tenant = new TenantRepository(_context, _logger);
        Currency = new CurrencyRepository(_context, _logger);
        PaymentMethod = new PaymentMethodRepository(_context, _logger);
        PaymentMethodCurrency = new PaymentMethodCurrencyRepository(_context, _logger);
        TenantPaymentMethod = new TenantPaymentMethodRepository(_context, _logger);
        TenantPaymentMethodCurrency = new TenantPaymentMethodCurrencyRepository(_context, _logger);
        PaymentTransaction = new PaymentTransactionRepository(_context, _logger);
        PaymentTransactionStatusHistory = new PaymentTransactionStatusHistoryRepository(_context, _logger);
        PaymentTransactionAudit = new PaymentTransactionAuditRepository(_context, _logger);
        WebhookEvent = new WebhookEventRepository(_context, _logger);
        WebhookDelivery = new WebhookDeliveryRepository(_context, _logger);
        RequestLog = new RequestLogRepository(_context, _logger);
        TenantCredential = new TenantCredentialRepository(_context, _logger);
    }

    public ITenantRepository Tenant { get; }
    public ICurrencyRepository Currency { get; }
    public IPaymentMethodRepository PaymentMethod { get; }
    public IPaymentMethodCurrencyRepository PaymentMethodCurrency { get; }
    public ITenantPaymentMethodRepository TenantPaymentMethod { get; }
    public ITenantPaymentMethodCurrencyRepository TenantPaymentMethodCurrency { get; }
    public IPaymentTransactionRepository PaymentTransaction { get; }
    public IPaymentTransactionStatusHistoryRepository PaymentTransactionStatusHistory { get; }
    public IPaymentTransactionAuditRepository PaymentTransactionAudit { get; }
    public IWebhookEventRepository WebhookEvent { get; }
    public IWebhookDeliveryRepository WebhookDelivery { get; }
    public IRequestLogRepository RequestLog { get; }
    public ITenantCredentialRepository TenantCredential { get; }

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<IDbContextTransaction> OpenTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
        return _transaction;
    }

    public async Task CommitAsync()
    {
        await _transaction.CommitAsync();
    }

    public async Task RollbackAsync()
    {
        await _transaction.RollbackAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}