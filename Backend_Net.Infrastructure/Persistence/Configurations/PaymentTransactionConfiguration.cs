using Backend_Net.Domain.Entities;
using Backend_Net.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend_Net.Infrastructure.Persistence.Configurations;

public class PaymentTransactionConfiguration : IEntityTypeConfiguration<PaymentTransaction>
{
    public void Configure(EntityTypeBuilder<PaymentTransaction> b)
    {
        b.ToTable("PaymentTransaction", "payment");

        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .HasDefaultValueSql("gen_random_uuid()");

        b.Property(x => x.Amount)
            .HasColumnType("numeric(20,8)");

        b.Property(x => x.NetAmount)
            .HasColumnType("numeric(20,8)");

        b.Property(x => x.Status)
            .HasConversion<int>()
            .IsRequired()
            .HasDefaultValue(PaymentStatus.Pending);

        b.Property(x => x.ProviderRawReq)
            .HasColumnType("jsonb");

        b.Property(x => x.ProviderRawRes)
            .HasColumnType("jsonb");

        b.Property(x => x.CallbackData)
            .HasColumnType("jsonb");

        b.Property(x => x.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("now()");

        b.Property(x => x.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("now()");

        b.HasOne(x => x.Tenant)
            .WithMany(t => t.PaymentTransactions)
            .HasForeignKey(x => x.TenantId);
        
        b.HasOne(x => x.TenantCredential)
            .WithMany(t => t.PaymentTransactions)
            .HasForeignKey(x => x.TenantCredentialId);

        b.HasOne(x => x.PaymentMethod)
            .WithMany(pm => pm.PaymentTransactions)
            .HasForeignKey(x => x.PaymentMethodId);

        b.HasOne(x => x.TenantCurrency)
            .WithMany(c => c.TenantPaymentTransactions)
            .HasForeignKey(x => x.TenantCurrencyCode);

        b.HasOne(x => x.UserCurrency)
            .WithMany(c => c.UserPaymentTransactions)
            .HasForeignKey(x => x.UserCurrencyCode);
        
        b.HasIndex(x => new { x.TenantId, x.CreatedAt })
            .HasDatabaseName("idx_payment_tx_tenant_created_at");

        b.HasIndex(x => new { x.TenantId, x.Status })
            .HasDatabaseName("idx_payment_tx_tenant_status");
    }
}
