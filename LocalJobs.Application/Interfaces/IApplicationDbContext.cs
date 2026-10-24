using Microsoft.EntityFrameworkCore;
using LocalJobs.Domain.Entities;

namespace LocalJobs.Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }

    DbSet<WorkerProfile> WorkerProfiles { get; }

    DbSet<Ad> Ads { get; }

    DbSet<AdCategory> AdCategories { get; }

    DbSet<Category> Categories { get; }

    DbSet<City> Cities { get; }

    DbSet<Job> Jobs { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
