using Backend_Net.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend_Net.Infrastructure.Persistence.Configurations;

public class PaymentMethodConfiguration : IEntityTypeConfiguration<PaymentMethod>
{
    public void Configure(EntityTypeBuilder<PaymentMethod> b)
    {
        b.ToTable("PaymentMethod", "payment");

        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .HasDefaultValueSql("gen_random_uuid()");

        b.Property(x => x.Code)
            .IsRequired();

        b.HasIndex(x => x.Code)
            .IsUnique();

        b.Property(x => x.DisplayName)
            .IsRequired();

        b.Property(x => x.BackgroundUrl);
        
        b.Property(x => x.PaymentMethodType)
            .IsRequired();
        
        b.Property(x => x.Type)
            .IsRequired();
        
        b.Property(x => x.RuleData)
            .IsRequired()
            .HasColumnType("jsonb");
        
        b.Property(x => x.RuleType)
            .IsRequired();
        
        b.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);
        
        b.Property(x => x.Fee)
            .IsRequired();
        
        b.Property(x => x.FeeType)
            .IsRequired();

        b.Property(x => x.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("now()");

        b.Property(x => x.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("now()");
    }
}