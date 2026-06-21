using LocalJobs.Application.Interfaces;
using LocalJobs.Contracts.Requests;
using LocalJobs.Contracts.Responses;
using LocalJobs.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LocalJobs.Application.Services;

public class JobService : IJobService
{
    private readonly IApplicationDbContext _context;

    public JobService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<JobResponse?> CreateJobAsync(CreateJobRequest request, Guid userId, CancellationToken cancellationToken = default)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException("UserId is required.");
        }

        var userExists = await _context.Users.AnyAsync(u => u.Id == userId, cancellationToken);
        if (!userExists)
        {
            throw new ArgumentException($"User with ID {userId} does not exist.");
        }

        var categoryExists = await _context.Categories.AnyAsync(c => c.Id == request.CategoryId, cancellationToken);
        if (!categoryExists)
        {
            throw new ArgumentException($"Category with ID {request.CategoryId} does not exist.");
        }

        var cityExists = await _context.Cities.AnyAsync(c => c.Id == request.CityId, cancellationToken);
        if (!cityExists)
        {
            throw new ArgumentException($"City with ID {request.CityId} does not exist.");
        }

        var job = new Job
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            Salary = request.Salary,
            CategoryId = request.CategoryId,
            CityId = request.CityId,
            UserId = userId,
            IsActive = true
        };

        _context.Jobs.Add(job);
        await _context.SaveChangesAsync(cancellationToken);

        return await GetJobByIdAsync(job.Id, cancellationToken);
    }

    public async Task<JobResponse?> GetJobByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Jobs
            .Where(j => j.Id == id && j.IsActive)
            .Select(j => new JobResponse(
                j.Id,
                j.Title,
                j.Description,
                j.Salary,
                j.CategoryId,
                j.Category.Name,
                j.CityId,
                j.City.Name,
                j.City.State,
                j.UserId,
                j.User.Name,
                j.IsActive,
                j.CreatedAt,
                j.UpdatedAt
            ))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<JobResponse>> GetJobsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Jobs
            .Where(j => j.IsActive)
            .OrderByDescending(j => j.CreatedAt)
            .Select(j => new JobResponse(
                j.Id,
                j.Title,
                j.Description,
                j.Salary,
                j.CategoryId,
                j.Category.Name,
                j.CityId,
                j.City.Name,
                j.City.State,
                j.UserId,
                j.User.Name,
                j.IsActive,
                j.CreatedAt,
                j.UpdatedAt
            ))
            .ToListAsync(cancellationToken);
    }

    public async Task<List<JobResponse>> SearchJobsAsync(int? cityId, int? categoryId, CancellationToken cancellationToken = default)
    {
        var query = _context.Jobs.Where(j => j.IsActive);

        if (cityId.HasValue)
        {
            query = query.Where(j => j.CityId == cityId.Value);
        }

        if (categoryId.HasValue)
        {
            query = query.Where(j => j.CategoryId == categoryId.Value);
        }

        return await query
            .OrderByDescending(j => j.CreatedAt)
            .Select(j => new JobResponse(
                j.Id,
                j.Title,
                j.Description,
                j.Salary,
                j.CategoryId,
                j.Category.Name,
                j.CityId,
                j.City.Name,
                j.City.State,
                j.UserId,
                j.User.Name,
                j.IsActive,
                j.CreatedAt,
                j.UpdatedAt
            ))
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> DeactivateJobAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var job = await _context.Jobs.FirstOrDefaultAsync(j => j.Id == id, cancellationToken);
        if (job == null)
        {
            return false;
        }

        job.IsActive = false;
        job.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
