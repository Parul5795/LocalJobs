using LocalJobs.Application.Interfaces;
using LocalJobs.Contracts.Requests;
using LocalJobs.Contracts.Responses;
using LocalJobs.Domain.Entities;
using LocalJobs.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace LocalJobs.Application.Services;

public class WorkerProfileService : IWorkerProfileService
{
    private readonly IApplicationDbContext _context;
    public WorkerProfileService(IApplicationDbContext context) => _context = context;

    public Task<WorkerProfileResponse?> GetMyProfileAsync(Guid userId, CancellationToken cancellationToken = default) =>
        ProfileQuery().FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);

    public async Task<WorkerProfileResponse> SaveMyProfileAsync(Guid userId, SaveWorkerProfileRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken)
            ?? throw new KeyNotFoundException("User was not found.");
        if (user.AccountType != AccountType.JobSeeker) throw new UnauthorizedAccessException("Only JobSeeker accounts can manage worker profiles.");
        if (request.ExperienceYears < 0 || request.ExperienceYears > 80 ||
            (request.IsFresher && request.ExperienceYears.GetValueOrDefault() > 0) ||
            string.IsNullOrWhiteSpace(request.Availability) || request.Availability.Length > 100 ||
            (request.ExperienceDetails?.Length ?? 0) > 1000 || (request.Skills?.Length ?? 0) > 2000 ||
            (request.Interests?.Length ?? 0) > 1000 || (request.About?.Length ?? 0) > 2000)
            throw new ArgumentException("Worker profile fields are invalid or exceed their allowed lengths.");
        if (!await _context.Categories.AnyAsync(c => c.Id == request.CategoryId, cancellationToken) ||
            !await _context.Cities.AnyAsync(c => c.Id == request.CityId, cancellationToken))
            throw new ArgumentException("A valid category and city are required.");

        var profile = await _context.WorkerProfiles.FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);
        if (profile is null)
        {
            profile = new WorkerProfile { Id = Guid.NewGuid(), UserId = userId };
            _context.WorkerProfiles.Add(profile);
        }
        profile.CategoryId = request.CategoryId; profile.CityId = request.CityId;
        profile.IsFresher = request.IsFresher; profile.ExperienceYears = request.ExperienceYears;
        profile.ExperienceDetails = request.ExperienceDetails?.Trim() ?? "";
        profile.Skills = request.Skills?.Trim() ?? ""; profile.Interests = request.Interests?.Trim() ?? "";
        profile.Availability = request.Availability.Trim(); profile.About = request.About?.Trim() ?? "";
        profile.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);
        return (await ProfileQuery().FirstAsync(p => p.UserId == userId, cancellationToken));
    }

    public Task<WorkerProfileResponse?> GetPublicProfileAsync(Guid userId, CancellationToken cancellationToken = default) =>
        ProfileQuery().FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);

    private IQueryable<WorkerProfileResponse> ProfileQuery() => _context.WorkerProfiles
        .Where(p => p.User.AccountType == AccountType.JobSeeker)
        .Select(p => new WorkerProfileResponse(p.UserId, p.User.Name, p.CategoryId, p.Category.Name,
            p.CityId, p.City.Name, p.City.State, p.IsFresher, p.ExperienceYears, p.ExperienceDetails,
            p.Skills, p.Interests, p.Availability, p.About, p.CreatedAt, p.UpdatedAt));
}
