using Backend_Net.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend_Net.Infrastructure.Persistence.Configurations;

public class TenantPaymentMethodCurrencyConfiguration : IEntityTypeConfiguration<TenantPaymentMethodCurrency>
{
    public void Configure(EntityTypeBuilder<TenantPaymentMethodCurrency> b)
    {
        b.ToTable("TenantPaymentMethodCurrency", "payment");

        b.HasKey(x => new { x.TenantId, x.PaymentMethodId, x.CurrencyCode });

        b.Property(x => x.MinAmount)
            .HasColumnType("numeric(20,8)");

        b.Property(x => x.MaxAmount)
            .HasColumnType("numeric(20,8)");

        b.Property(x => x.FeeValue)
            .HasColumnType("numeric(20,8)");

        b.Property(x => x.IsEnabled)
            .IsRequired()
            .HasDefaultValue(true);

        b.HasOne(x => x.Tenant)
            .WithMany(t => t.TenantPaymentMethodCurrencies)
            .HasForeignKey(x => x.TenantId);

        b.HasOne(x => x.PaymentMethod)
            .WithMany(pm => pm.TenantPaymentMethodCurrencies)
            .HasForeignKey(x => x.PaymentMethodId);

        b.HasOne(x => x.Currency)
            .WithMany(c => c.TenantPaymentMethodCurrencies)
            .HasForeignKey(x => x.CurrencyCode);

        b.HasOne(x => x.GlobalPaymentMethodCurrency)
            .WithMany(g => g.TenantPaymentMethodCurrencies)
            .HasForeignKey(x => new { x.PaymentMethodId, x.CurrencyCode })
            .HasPrincipalKey(x => new { x.PaymentMethodId, x.CurrencyCode })
            .HasConstraintName("fk_global_method_currency");
    }
}