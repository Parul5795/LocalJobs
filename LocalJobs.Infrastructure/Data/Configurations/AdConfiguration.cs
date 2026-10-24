using LocalJobs.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LocalJobs.Infrastructure.Data.Configurations;

public class AdConfiguration : IEntityTypeConfiguration<Ad>
{
    public void Configure(EntityTypeBuilder<Ad> builder)
    {
        builder.ToTable("Ads");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Title).IsRequired().HasMaxLength(120);
        builder.Property(a => a.Description).IsRequired().HasMaxLength(3000);
        builder.Property(a => a.Price).HasPrecision(18, 2);
        builder.Property(a => a.IsActive).HasDefaultValue(true);
        builder.Property(a => a.Area).HasMaxLength(100);
        builder.Property(a => a.ImageUrls).HasColumnType("text[]").HasDefaultValueSql("ARRAY[]::text[]");
        builder.HasIndex(a => new { a.IsActive, a.CreatedAt });
        builder.HasIndex(a => new { a.CityId, a.AdCategoryId, a.IsActive, a.CreatedAt });
        builder.HasIndex(a => new { a.CityId, a.Area, a.IsActive });
        builder.HasOne(a => a.AdCategory).WithMany(c => c.Ads).HasForeignKey(a => a.AdCategoryId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(a => a.City).WithMany().HasForeignKey(a => a.CityId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(a => a.User).WithMany().HasForeignKey(a => a.UserId).OnDelete(DeleteBehavior.Restrict);
    }
}
