using Backend_Net.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend_Net.Infrastructure.Persistence.Configurations;

public class PaymentTransactionStatusHistoryConfiguration : IEntityTypeConfiguration<PaymentTransactionStatusHistory>
{
    public void Configure(EntityTypeBuilder<PaymentTransactionStatusHistory> b)
    {
        b.ToTable("PaymentTransactionStatusHistory", "payment");

        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .HasDefaultValueSql("gen_random_uuid()");

        b.Property(x => x.FromStatus)
            .HasConversion<int?>();

        b.Property(x => x.ToStatus)
            .HasConversion<int>()
            .IsRequired();

        b.Property(x => x.ChangedAt)
            .IsRequired()
            .HasDefaultValueSql("now()");

        b.HasOne(x => x.Transaction)
            .WithMany(t => t.StatusHistories)
            .HasForeignKey(x => x.TransactionId);

        b.HasIndex(x => new { x.TransactionId, x.ChangedAt })
            .HasDatabaseName("idx_payment_tx_status_hist_tx");
    }
}