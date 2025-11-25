using Backend_Net.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend_Net.Infrastructure.Persistence.Configurations
{
    public class TenantCredentialConfiguration : IEntityTypeConfiguration<TenantCredential>
    {
        public void Configure(EntityTypeBuilder<TenantCredential> builder)
        {
            builder.ToTable("TenantCredentials", "payment");

            builder.HasKey(x => x.Id);
            
            builder.Property(x => x.Id)
                .HasDefaultValueSql("gen_random_uuid()");
            
            builder.Property(x => x.TenantId)
                .IsRequired();

            builder.Property(x => x.ApiKey)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.SecretEncrypted)
                .HasColumnType("text")
                .IsRequired();

            builder.Property(x => x.Status)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .HasDefaultValueSql("NOW()")
                .IsRequired();

            builder.Property(x => x.ExpiredAt);

            builder.Property(x => x.LastRotatedAt)
                .HasDefaultValueSql("NOW()")
                .IsRequired();

            builder
                .HasOne(x => x.Tenant)
                .WithMany(x => x.Credentials)
                .HasForeignKey(x => x.TenantId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.ApiKey)
                .HasDatabaseName("idx_tenant_credentials_apikey");

            builder.HasIndex(x => new { x.TenantId, x.Status })
                .HasDatabaseName("idx_tenant_credentials_tenant_status");
        }
    }
}
