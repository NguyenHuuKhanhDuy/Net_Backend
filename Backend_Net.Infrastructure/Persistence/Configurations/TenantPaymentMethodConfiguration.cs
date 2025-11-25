using Backend_Net.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend_Net.Infrastructure.Persistence.Configurations;

public class TenantPaymentMethodConfiguration : IEntityTypeConfiguration<TenantPaymentMethod>
{
    public void Configure(EntityTypeBuilder<TenantPaymentMethod> b)
    {
        b.ToTable("TenantPaymentMethod", "payment");

        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .HasDefaultValueSql("gen_random_uuid()");

        b.Property(x => x.IsEnabled)
            .IsRequired()
            .HasDefaultValue(true);

        b.Property(x => x.ConfigJson)
            .HasColumnType("jsonb");

        b.Property(x => x.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("now()");

        b.Property(x => x.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("now()");

        b.HasIndex(x => new { x.TenantId, x.PaymentMethodId })
            .IsUnique();

        b.HasOne(x => x.Tenant)
            .WithMany(t => t.TenantPaymentMethods)
            .HasForeignKey(x => x.TenantId);

        b.HasOne(x => x.PaymentMethod)
            .WithMany(pm => pm.TenantPaymentMethods)
            .HasForeignKey(x => x.PaymentMethodId);
    }
}