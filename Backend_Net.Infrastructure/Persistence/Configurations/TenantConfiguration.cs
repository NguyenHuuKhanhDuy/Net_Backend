using Backend_Net.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend_Net.Infrastructure.Persistence.Configurations;

public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> b)
    {
        b.ToTable("Tenant", "payment");

        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .HasDefaultValueSql("gen_random_uuid()");

        b.Property(x => x.Code)
            .IsRequired();

        b.HasIndex(x => x.Code)
            .IsUnique();

        b.Property(x => x.Name)
            .IsRequired();

        b.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        b.Property(x => x.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("now()");

        b.Property(x => x.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("now()");
    }
}