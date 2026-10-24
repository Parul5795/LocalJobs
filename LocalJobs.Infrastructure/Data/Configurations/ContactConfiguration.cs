using LocalJobs.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LocalJobs.Infrastructure.Data.Configurations;

public class ContactConfiguration : IEntityTypeConfiguration<Contact>
{
    public void Configure(EntityTypeBuilder<Contact> builder)
    {
        builder.ToTable("Contacts");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Method).HasConversion<string>().HasMaxLength(20);
        builder.HasIndex(c => new { c.WorkerUserId, c.CreatedAt });
        builder.HasIndex(c => new { c.EmployerUserId, c.CreatedAt });
        builder.HasOne<User>().WithMany().HasForeignKey(c => c.WorkerUserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>().WithMany().HasForeignKey(c => c.EmployerUserId).OnDelete(DeleteBehavior.Restrict);
    }
}
