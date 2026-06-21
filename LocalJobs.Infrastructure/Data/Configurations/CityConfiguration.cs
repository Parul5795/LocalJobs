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
            new City { Id = 1, Name = "Pune", State = "Maharashtra" },
            new City { Id = 2, Name = "Mumbai", State = "Maharashtra" },
            new City { Id = 3, Name = "Bangalore", State = "Karnataka" },
            new City { Id = 4, Name = "Delhi", State = "Delhi" },
            new City { Id = 5, Name = "Hyderabad", State = "Telangana" },
            new City { Id = 6, Name = "Chennai", State = "Tamil Nadu" }
        );
    }
}
