using Backend_Net.Application.Common.Interfaces.Repositories;
using Backend_Net.Infrastructure.Persistence.Repository;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace Backend_Net.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork, IAsyncDisposable, IDisposable
{
    private readonly AppDbContext _context;
    private IDbContextTransaction? _transaction;
    private readonly ILogger _logger;

    public UnitOfWork(AppDbContext context, ILoggerFactory loggerFactory)
    {
        _context = context;
        _logger = loggerFactory.CreateLogger("UnitOfWork");

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
        Country = new CountryRepository(_context, _logger);
        CurrencyRate = new CurrencyRateRepository(_context, _logger);
    }

    public ITenantRepository Tenant { get; }
    public ICurrencyRepository Currency { get; }
    public ICurrencyRateRepository CurrencyRate { get; }
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
    public ICountryRepository Country { get; }

    public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
    
    public async Task<IDbContextTransaction> OpenTransactionAsync(
        CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database
            .BeginTransactionAsync(cancellationToken)
            .ConfigureAwait(false);

        return _transaction;
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null)
            throw new InvalidOperationException("No active transaction");

        await _transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        if (_transaction != null)
        {
            await _transaction.DisposeAsync().ConfigureAwait(false);
        }

        await _context.DisposeAsync().ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }
}
