using Backend_Net.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend_Net.Infrastructure.Persistence.Configurations;

public class CountryConfiguration : IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Country> builder)
    {
        builder.ToTable("Country", "payment");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.FlagUrl)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.DialCode)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(x => x.Alpha2Code)
            .IsRequired()
            .HasMaxLength(2)
            .IsFixedLength();

        builder.Property(x => x.Alpha3Code)
            .IsRequired()
            .HasMaxLength(3)
            .IsFixedLength();
        
        builder.Property(x => x.CurrencyCode)
            .IsRequired()
            .HasMaxLength(10);

        builder.HasIndex(x => x.Alpha2Code).IsUnique();
        builder.HasIndex(x => x.Alpha3Code).IsUnique();
        builder.HasIndex(x => x.DialCode);
        builder.HasOne(x => x.BaseCurrency)
            .WithMany(c => c.Countries)
            .HasForeignKey(x => x.CurrencyCode)
            .OnDelete(DeleteBehavior.Restrict);
    }
}