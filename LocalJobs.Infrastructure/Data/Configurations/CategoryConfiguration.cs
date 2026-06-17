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
            new Category { Id = 1, Name = "IT & Software Development" },
            new Category { Id = 2, Name = "Healthcare & Medicine" },
            new Category { Id = 3, Name = "Education & Training" },
            new Category { Id = 4, Name = "Construction & Trades" },
            new Category { Id = 5, Name = "Retail & Customer Service" },
            new Category { Id = 6, Name = "Hospitality & Tourism" },
            new Category { Id = 7, Name = "Finance & Accounting" }
        );
    }
}
