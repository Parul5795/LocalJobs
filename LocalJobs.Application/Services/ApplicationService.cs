using LocalJobs.Application.Interfaces;
using LocalJobs.Contracts.Requests;
using LocalJobs.Contracts.Responses;
using LocalJobs.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LocalJobs.Application.Services;

public class ApplicationService : IApplicationService
{
    private readonly IApplicationDbContext _context;

    public ApplicationService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ApplicationResponse> ApplyForJobAsync(Guid jobId, Guid userId, ApplyForJobRequest request, CancellationToken cancellationToken = default)
    {
        if (jobId == Guid.Empty)
        {
            throw new ArgumentException("JobId is required.");
        }

        if (userId == Guid.Empty)
        {
            throw new ArgumentException("UserId is required.");
        }

        var job = await _context.Jobs.FirstOrDefaultAsync(j => j.Id == jobId, cancellationToken);
        if (job is null)
        {
            throw new KeyNotFoundException($"Job with ID '{jobId}' was not found.");
        }

        if (!job.IsActive)
        {
            throw new InvalidOperationException("You cannot apply to an inactive job.");
        }

        var applicant = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (applicant is null)
        {
            throw new KeyNotFoundException($"User with ID '{userId}' was not found.");
        }

        var alreadyApplied = await _context.Applications
            .AnyAsync(a => a.JobId == jobId && a.UserId == userId, cancellationToken);

        if (alreadyApplied)
        {
            throw new InvalidOperationException("You have already applied for this job.");
        }

        var application = new Domain.Entities.Application
        {
            Id = Guid.NewGuid(),
            JobId = jobId,
            UserId = userId,
            CoverLetter = request.CoverLetter.Trim(),
            Status = Domain.Enums.ApplicationStatus.Applied
        };

        _context.Applications.Add(application);
        await _context.SaveChangesAsync(cancellationToken);

        return new ApplicationResponse(
            application.Id,
            application.JobId,
            job.Title,
            application.UserId,
            applicant.Name,
            application.CoverLetter,
            application.Status.ToString(),
            application.CreatedAt);
    }

    public async Task<List<ApplicationResponse>> GetMyApplicationsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException("UserId is required.");
        }

        return await _context.Applications
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => new ApplicationResponse(
                a.Id,
                a.JobId,
                a.Job.Title,
                a.UserId,
                a.User.Name,
                a.CoverLetter,
                a.Status.ToString(),
                a.CreatedAt))
            .ToListAsync(cancellationToken);
    }
}
