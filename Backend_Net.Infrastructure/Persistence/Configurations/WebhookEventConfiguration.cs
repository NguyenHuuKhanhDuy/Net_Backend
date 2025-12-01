using Backend_Net.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend_Net.Infrastructure.Persistence.Configurations;

public class WebhookEventConfiguration : IEntityTypeConfiguration<WebhookEvent>
{
    public void Configure(EntityTypeBuilder<WebhookEvent> b)
    {
        b.ToTable("WebhookEvent", "payment");

        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .HasDefaultValueSql("gen_random_uuid()");

        b.Property(x => x.EventType)
            .IsRequired();

        b.Property(x => x.CallbackUrl)
            .IsRequired();
        
        b.Property(x => x.ApiKey)
            .IsRequired();
        
        b.Property(x => x.Signature)
            .IsRequired();

        b.Property(x => x.Payload)
            .IsRequired()
            .HasColumnType("jsonb");

        b.Property(x => x.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("now()");

        b.HasOne(x => x.Tenant)
            .WithMany(t => t.WebhookEvents)
            .HasForeignKey(x => x.TenantId);

        b.HasOne(x => x.Transaction)
            .WithMany(t => t.WebhookEvents)
            .HasForeignKey(x => x.TransactionId);
    }
}