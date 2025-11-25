using System.Reflection;
using Backend_Net.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend_Net.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<Currency> Currencies => Set<Currency>();
    public DbSet<PaymentMethod> PaymentMethods => Set<PaymentMethod>();
    public DbSet<PaymentMethodCurrency> PaymentMethodCurrencies => Set<PaymentMethodCurrency>();
    public DbSet<PaymentTransactionAudit> PaymentTransactionAudits => Set<PaymentTransactionAudit>();
    public DbSet<PaymentTransaction> PaymentTransactions => Set<PaymentTransaction>();
    public DbSet<PaymentTransactionStatusHistory> PaymentTransactionStatusHistories => Set<PaymentTransactionStatusHistory>();
    public DbSet<RequestLog> RequestLogs => Set<RequestLog>();
    public DbSet<TenantPaymentMethod> TenantPaymentMethods => Set<TenantPaymentMethod>();
    public DbSet<TenantPaymentMethodCurrency> TenantPaymentMethodCurrencies => Set<TenantPaymentMethodCurrency>();
    public DbSet<WebhookDelivery> WebhookDeliveries => Set<WebhookDelivery>();
    public DbSet<WebhookEvent> WebhookEvents => Set<WebhookEvent>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}