using Microsoft.EntityFrameworkCore;
using LocalJobs.Domain.Entities;

namespace LocalJobs.Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }

    DbSet<Category> Categories { get; }

    DbSet<City> Cities { get; }

    DbSet<Job> Jobs { get; }

    DbSet<LocalJobs.Domain.Entities.Application> Applications { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
