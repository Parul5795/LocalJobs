using LocalJobs.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LocalJobs.Infrastructure.Data.Configurations;

public class WorkerProfileConfiguration : IEntityTypeConfiguration<WorkerProfile>
{
    public void Configure(EntityTypeBuilder<WorkerProfile> builder)
    {
        builder.ToTable("WorkerProfiles");
        builder.HasKey(p => p.Id);
        builder.HasIndex(p => p.UserId).IsUnique();
        builder.HasIndex(p => new { p.CategoryId, p.CityId, p.IsFresher });
        builder.Property(p => p.ExperienceYears).HasPrecision(5, 2);
        builder.Property(p => p.ExperienceDetails).HasMaxLength(1000).HasDefaultValue("");
        builder.Property(p => p.Skills).HasMaxLength(2000).HasDefaultValue("");
        builder.Property(p => p.Interests).HasMaxLength(1000).HasDefaultValue("");
        builder.Property(p => p.Availability).IsRequired().HasMaxLength(100).HasDefaultValue("");
        builder.Property(p => p.About).HasMaxLength(2000).HasDefaultValue("");
        builder.HasOne(p => p.Category).WithMany().HasForeignKey(p => p.CategoryId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(p => p.City).WithMany().HasForeignKey(p => p.CityId).OnDelete(DeleteBehavior.Restrict);
    }
}
