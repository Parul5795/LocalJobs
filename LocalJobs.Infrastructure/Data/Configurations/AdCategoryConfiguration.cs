using LocalJobs.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LocalJobs.Infrastructure.Data.Configurations;

public class AdCategoryConfiguration : IEntityTypeConfiguration<AdCategory>
{
    public void Configure(EntityTypeBuilder<AdCategory> builder)
    {
        builder.ToTable("AdCategories");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedOnAdd();
        builder.Property(c => c.Name).IsRequired().HasMaxLength(50);
        builder.HasIndex(c => c.Name).IsUnique();
        builder.HasData(
            new AdCategory { Id = 1, Name = "Jobs/Work" },
            new AdCategory { Id = 2, Name = "Services" },
            new AdCategory { Id = 3, Name = "Business/Shop" },
            new AdCategory { Id = 4, Name = "Products for Sale" },
            new AdCategory { Id = 5, Name = "Tuition" },
            new AdCategory { Id = 6, Name = "House/Room" },
            new AdCategory { Id = 7, Name = "Vehicle" },
            new AdCategory { Id = 8, Name = "Local Announcement" },
            new AdCategory { Id = 9, Name = "Other" });
    }
}
