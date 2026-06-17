using LocalJobs.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LocalJobs.Infrastructure.Data.Configurations;

public class JobConfiguration : IEntityTypeConfiguration<Job>
{
    public void Configure(EntityTypeBuilder<Job> builder)
    {
        builder.ToTable("Jobs");

        builder.HasKey(j => j.Id);

        builder.Property(j => j.Title)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(j => j.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(j => j.Salary)
            .HasPrecision(18, 2);

        builder.Property(j => j.IsActive)
            .HasDefaultValue(true);

        // Relationships
        builder.HasOne(j => j.Category)
            .WithMany(c => c.Jobs)
            .HasForeignKey(j => j.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(j => j.City)
            .WithMany(c => c.Jobs)
            .HasForeignKey(j => j.CityId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(j => j.User)
            .WithMany(u => u.PostedJobs)
            .HasForeignKey(j => j.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
