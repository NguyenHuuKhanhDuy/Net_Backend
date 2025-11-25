using Backend_Net.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend_Net.Infrastructure.Persistence.Configurations;

public class CurrencyConfiguration : IEntityTypeConfiguration<Currency>
{
    public void Configure(EntityTypeBuilder<Currency> b)
    {
        b.ToTable("Currency", "payment");

        b.HasKey(x => x.Code);

        b.Property(x => x.Code)
            .HasColumnType("varchar(10)");

        b.Property(x => x.Name)
            .IsRequired();

        b.Property(x => x.MinorUnit)
            .IsRequired()
            .HasDefaultValue(2);

        b.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);
    }
}