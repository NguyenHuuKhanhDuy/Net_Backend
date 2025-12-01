using Backend_Net.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend_Net.Infrastructure.Persistence.Configurations;

public class CurrencyRateConfiguration : IEntityTypeConfiguration<CurrencyRate>
{
    public void Configure(EntityTypeBuilder<CurrencyRate> builder)
    {
        builder.ToTable("CurrencyRate", "payment");

        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Id)
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(x => x.BaseCurrencyCode)
            .HasColumnType("varchar(10)")
            .IsRequired();

        builder.Property(x => x.QuoteCurrencyCode)
            .HasColumnType("varchar(10)")
            .IsRequired();

        builder.Property(x => x.Rate)
            .HasPrecision(20, 8)
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .HasDefaultValueSql("NOW()")
            .IsRequired();

        // Relations
        builder.HasOne(x => x.BaseCurrency)
            .WithMany(c => c.BaseCurrencyRates)
            .HasForeignKey(x => x.BaseCurrencyCode)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.QuoteCurrency)
            .WithMany(c => c.QuoteCurrencyRates)
            .HasForeignKey(x => x.QuoteCurrencyCode)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.BaseCurrencyCode, x.QuoteCurrencyCode })
            .IsUnique()
            .HasDatabaseName("uq_currency_rate");
    }
}