using LocalJobs.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LocalJobs.Infrastructure.Data.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .ValueGeneratedOnAdd();

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(c => c.Name)
            .IsUnique();

        builder.HasData(
            new Category { Id = 1, Name = "Driver" },
            new Category { Id = 2, Name = "Maid" },
            new Category { Id = 3, Name = "Cook" },
            new Category { Id = 4, Name = "Tutor" },
            new Category { Id = 5, Name = "Electrician" },
            new Category { Id = 6, Name = "Plumber" },
            new Category { Id = 7, Name = "Delivery Boy" },
            new Category { Id = 8, Name = "Office Assistant" }
        );
    }
}
