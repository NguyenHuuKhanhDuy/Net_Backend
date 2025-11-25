using Backend_Net.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend_Net.Infrastructure.Persistence.Configurations;

public class PaymentMethodCurrencyConfiguration : IEntityTypeConfiguration<PaymentMethodCurrency>
{
    public void Configure(EntityTypeBuilder<PaymentMethodCurrency> b)
    {
        b.ToTable("PaymentMethodCurrency", "payment");

        b.HasKey(x => new { x.PaymentMethodId, x.CurrencyCode });

        b.Property(x => x.MinAmount)
            .HasColumnType("numeric(20,8)");

        b.Property(x => x.MaxAmount)
            .HasColumnType("numeric(20,8)");

        b.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        b.HasOne(x => x.PaymentMethod)
            .WithMany(pm => pm.PaymentMethodCurrencies)
            .HasForeignKey(x => x.PaymentMethodId);

        b.HasOne(x => x.Currency)
            .WithMany(c => c.PaymentMethodCurrencies)
            .HasForeignKey(x => x.CurrencyCode);
    }
}