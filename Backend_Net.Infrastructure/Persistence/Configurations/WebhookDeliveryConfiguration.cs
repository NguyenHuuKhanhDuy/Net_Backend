using Backend_Net.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend_Net.Infrastructure.Persistence.Configurations;

public class WebhookDeliveryConfiguration : IEntityTypeConfiguration<WebhookDelivery>
{
    public void Configure(EntityTypeBuilder<WebhookDelivery> b)
    {
        b.ToTable("WebhookDelivery", "payment");

        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .HasDefaultValueSql("gen_random_uuid()");

        b.Property(x => x.Attempt)
            .IsRequired()
            .HasDefaultValue(1);

        b.Property(x => x.Status)
            .IsRequired();

        b.Property(x => x.RequestBody)
            .HasColumnType("jsonb");

        b.Property(x => x.ResponseBody);

        b.Property(x => x.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("now()");

        b.HasOne(x => x.Event)
            .WithMany(e => e.Deliveries)
            .HasForeignKey(x => x.EventId);

        b.HasIndex(x => x.EventId)
            .HasDatabaseName("idx_webhook_delivery_event");
    }
}