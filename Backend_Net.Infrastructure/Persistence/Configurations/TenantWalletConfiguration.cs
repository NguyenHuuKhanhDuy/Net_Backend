using Backend_Net.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend_Net.Infrastructure.Persistence.Configurations;

public class TenantWalletConfiguration : IEntityTypeConfiguration<TenantWallet>
{
    public void Configure(EntityTypeBuilder<TenantWallet> builder)
    {
        builder.ToTable("TenantWallet", "payment");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TenantId)
            .IsRequired();

        builder.Property(x => x.CurrencyCode)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(x => x.Balance)
            .HasColumnType("numeric(20,8)")
            .HasDefaultValue(0);

        builder.Property(x => x.PendingBalance)
            .HasColumnType("numeric(20,8)")
            .HasDefaultValue(0);

        builder.Property(x => x.IsActive)
            .HasDefaultValue(true);

        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("NOW()");

        builder.Property(x => x.UpdatedAt)
            .HasDefaultValueSql("NOW()");

        builder.HasIndex(x => new { x.TenantId, x.CurrencyCode })
            .IsUnique();

        builder.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.TenantId);

        builder.HasOne(x => x.Currency)
            .WithMany()
            .HasForeignKey(x => x.CurrencyCode);
    }
}