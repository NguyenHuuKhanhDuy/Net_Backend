using Backend_Net.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend_Net.Infrastructure.Persistence.Configurations;

public class TenantWalletAuditConfiguration : IEntityTypeConfiguration<TenantWalletAudit>
{
    public void Configure(EntityTypeBuilder<TenantWalletAudit> builder)
    {
        builder.ToTable("TenantWalletAudit", "payment");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.WalletId)
            .IsRequired();

        builder.Property(x => x.Amount)
            .HasColumnType("numeric(20,8)")
            .IsRequired();

        builder.Property(x => x.BalanceBefore)
            .HasColumnType("numeric(20,8)")
            .IsRequired();

        builder.Property(x => x.BalanceAfter)
            .HasColumnType("numeric(20,8)")
            .IsRequired();

        builder.Property(x => x.Direction)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("NOW()");

        builder.HasIndex(x => x.WalletId);
        builder.HasIndex(x => x.TransactionId);
        builder.HasIndex(x => x.RelatedType);

        builder.HasOne(x => x.Wallet)
            .WithMany(x => x.Audits)
            .HasForeignKey(x => x.WalletId);
    }
}