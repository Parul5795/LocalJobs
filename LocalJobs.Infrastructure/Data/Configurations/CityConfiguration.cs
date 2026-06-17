using LocalJobs.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LocalJobs.Infrastructure.Data.Configurations;

public class CityConfiguration : IEntityTypeConfiguration<City>
{
    public void Configure(EntityTypeBuilder<City> builder)
    {
        builder.ToTable("Cities");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .ValueGeneratedOnAdd();

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.State)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(c => new { c.Name, c.State })
            .IsUnique();

        builder.HasData(
            new City { Id = 1, Name = "New York", State = "NY" },
            new City { Id = 2, Name = "Los Angeles", State = "CA" },
            new City { Id = 3, Name = "Chicago", State = "IL" },
            new City { Id = 4, Name = "Houston", State = "TX" },
            new City { Id = 5, Name = "Phoenix", State = "AZ" },
            new City { Id = 6, Name = "Philadelphia", State = "PA" },
            new City { Id = 7, Name = "Seattle", State = "WA" }
        );
    }
}
