using LocalJobs.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LocalJobs.Infrastructure.Data.Configurations;

public class ApplicationConfiguration : IEntityTypeConfiguration<LocalJobs.Domain.Entities.Application>
{
    public void Configure(EntityTypeBuilder<LocalJobs.Domain.Entities.Application> builder)
    {
        builder.ToTable("Applications");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.CoverLetter)
            .HasMaxLength(1000);

        builder.Property(a => a.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        // Relationships
        builder.HasOne(a => a.Job)
            .WithMany(j => j.Applications)
            .HasForeignKey(a => a.JobId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.User)
            .WithMany(u => u.Applications)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
