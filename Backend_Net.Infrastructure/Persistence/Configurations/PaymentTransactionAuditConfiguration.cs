using Backend_Net.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend_Net.Infrastructure.Persistence.Configurations;

public class PaymentTransactionAuditConfiguration : IEntityTypeConfiguration<PaymentTransactionAudit>
{
    public void Configure(EntityTypeBuilder<PaymentTransactionAudit> b)
    {
        b.ToTable("PaymentTransactionAudit", "payment");

        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .HasDefaultValueSql("gen_random_uuid()");

        b.Property(x => x.FieldName)
            .IsRequired();

        b.Property(x => x.ChangedAt)
            .IsRequired()
            .HasDefaultValueSql("now()");

        b.HasOne(x => x.Transaction)
            .WithMany(t => t.Audits)
            .HasForeignKey(x => x.TransactionId);

        b.HasOne(x => x.Tenant)
            .WithMany(t => t.PaymentTransactionAudits)
            .HasForeignKey(x => x.TenantId);

        b.HasIndex(x => new { x.TransactionId, x.ChangedAt })
            .HasDatabaseName("idx_payment_tx_audit_tx");

        b.HasIndex(x => new { x.TenantId, x.ChangedAt })
            .HasDatabaseName("idx_payment_tx_audit_tenant");
    }
}