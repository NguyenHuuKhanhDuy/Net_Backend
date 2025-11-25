using Backend_Net.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend_Net.Infrastructure.Persistence.Configurations;

public class RequestLogConfiguration : IEntityTypeConfiguration<RequestLog>
{
    public void Configure(EntityTypeBuilder<RequestLog> b)
    {
        b.ToTable("RequestLog", "payment");

        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .HasDefaultValueSql("gen_random_uuid()");

        b.Property(x => x.TenantCredentialId)
            .IsRequired();

        b.Property(x => x.Path)
            .IsRequired();

        b.Property(x => x.Method)
            .IsRequired();

        b.Property(x => x.Headers)
            .HasColumnType("jsonb");

        b.Property(x => x.QueryParams)
            .HasColumnType("jsonb");

        b.Property(x => x.Body)
            .HasColumnType("jsonb");

        b.Property(x => x.ResponseBody)
            .HasColumnType("jsonb");

        b.Property(x => x.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("now()");

        b.HasOne(x => x.TenantCredential)
            .WithMany(t => t.RequestLogs)
            .HasForeignKey(x => x.TenantCredentialId);

        b.HasIndex(x => x.CreatedAt)
            .HasDatabaseName("idx_request_log_created_at");

        b.HasIndex(x => x.Path)
            .HasDatabaseName("idx_request_log_path");

        b.HasIndex(x => x.TenantCredentialId)
            .HasDatabaseName("idx_request_log_tenant_credential");
    }
}