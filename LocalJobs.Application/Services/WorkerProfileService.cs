using LocalJobs.Application.Interfaces;
using LocalJobs.Contracts.Requests;
using LocalJobs.Contracts.Responses;
using LocalJobs.Domain.Entities;
using LocalJobs.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace LocalJobs.Application.Services;

public class WorkerProfileService(IApplicationDbContext context) : IWorkerProfileService
{
    public Task<WorkerProfileResponse?> GetMyProfileAsync(Guid userId, CancellationToken cancellationToken = default) => ProfileQuery(userId).FirstOrDefaultAsync(cancellationToken);

    public async Task<WorkerProfileResponse> SaveMyProfileAsync(Guid userId, SaveWorkerProfileRequest request, CancellationToken cancellationToken = default)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken) ?? throw new KeyNotFoundException("User was not found.");
        if (user.AccountType != AccountType.JobSeeker) throw new UnauthorizedAccessException("Only JobSeeker accounts can manage worker profiles.");
        if (request.CategoryId <= 0 || request.CityId <= 0 || request.ExperienceYears is < 0 or > 80 ||
            (request.IsFresher && request.ExperienceYears.GetValueOrDefault() > 0) || (request.Availability?.Length ?? 0) > 100 ||
            (request.ExperienceDetails?.Length ?? 0) > 1000 || (request.Skills?.Length ?? 0) > 2000 ||
            (request.Interests?.Length ?? 0) > 1000 || (request.About?.Length ?? 0) > 2000 || (request.Area?.Length ?? 0) > 100 ||
            !ValidImageUrl(request.ProfileImageUrl))
            throw new ArgumentException("Worker profile fields are invalid or exceed their allowed lengths.");
        if (!await context.Categories.AnyAsync(c => c.Id == request.CategoryId, cancellationToken) || !await context.Cities.AnyAsync(c => c.Id == request.CityId, cancellationToken))
            throw new ArgumentException("A valid category and city are required.");

        var profile = await context.WorkerProfiles.FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);
        if (profile is null) { profile = new WorkerProfile { Id = Guid.NewGuid(), UserId = userId }; context.WorkerProfiles.Add(profile); }
        profile.CategoryId = request.CategoryId; profile.CityId = request.CityId; profile.Area = TrimOrNull(request.Area);
        profile.ProfileImageUrl = TrimOrNull(request.ProfileImageUrl); profile.IsFresher = request.IsFresher; profile.ExperienceYears = request.ExperienceYears;
        profile.ExperienceDetails = request.ExperienceDetails?.Trim() ?? ""; profile.Skills = request.Skills?.Trim() ?? "";
        profile.Interests = request.Interests?.Trim() ?? ""; profile.Availability = request.Availability?.Trim() ?? "";
        profile.About = request.About?.Trim() ?? ""; profile.UpdatedAt = DateTime.UtcNow;
        await context.SaveChangesAsync(cancellationToken);
        return await ProfileQuery(userId).FirstAsync(cancellationToken);
    }

    public Task<WorkerProfileResponse?> GetPublicProfileAsync(Guid userId, CancellationToken cancellationToken = default) => ProfileQuery(userId).FirstOrDefaultAsync(cancellationToken);

    public async Task<PagedResponse<WorkerSearchResultResponse>> SearchWorkersAsync(int? categoryId, int? cityId, string? area, bool? isFresher,
        string? keyword, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        ValidatePage(page, pageSize);
        if (categoryId is <= 0 || cityId is <= 0 || (area?.Length ?? 0) > 100 || (keyword?.Length ?? 0) > 100)
            throw new ArgumentException("Search filters are invalid or exceed their allowed lengths.");
        var query = context.WorkerProfiles.Where(p => p.User.AccountType == AccountType.JobSeeker);
        if (categoryId.HasValue) query = query.Where(p => p.CategoryId == categoryId.Value);
        if (cityId.HasValue) query = query.Where(p => p.CityId == cityId.Value);
        if (!string.IsNullOrWhiteSpace(area)) { var term = area.Trim().ToLower(); query = query.Where(p => p.Area != null && p.Area.ToLower().Contains(term)); }
        if (isFresher.HasValue) query = query.Where(p => p.IsFresher == isFresher.Value);
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var term = keyword.Trim().ToLower();
            query = query.Where(p => p.User.Name.ToLower().Contains(term) || p.Skills.ToLower().Contains(term) ||
                p.Interests.ToLower().Contains(term) || p.About.ToLower().Contains(term) || p.ExperienceDetails.ToLower().Contains(term));
        }
        var count = await query.CountAsync(cancellationToken);
        var items = await SearchProjection(query.OrderBy(p => p.User.Name).ThenBy(p => p.UserId)
            .Skip((page - 1) * pageSize).Take(pageSize)).ToListAsync(cancellationToken);
        return new PagedResponse<WorkerSearchResultResponse>(items, page, pageSize, count, (int)Math.Ceiling(count / (double)pageSize));
    }

    private IQueryable<WorkerProfileResponse> ProfileQuery(Guid userId) => context.WorkerProfiles
        .Where(p => p.UserId == userId && p.User.AccountType == AccountType.JobSeeker)
        .Select(p => new WorkerProfileResponse(p.UserId, p.User.Name, p.CategoryId, p.Category.Name, p.CityId, p.City.Name,
            p.City.State, p.IsFresher, p.ExperienceYears, p.ExperienceDetails, p.Skills, p.Interests, p.Availability, p.About,
            p.CreatedAt, p.UpdatedAt, p.Area, p.ProfileImageUrl, p.ContactCount));

    private static IQueryable<WorkerSearchResultResponse> SearchProjection(IQueryable<WorkerProfile> query) => query.Select(p =>
        new WorkerSearchResultResponse(p.UserId, p.User.Name, p.CategoryId, p.Category.Name, p.CityId, p.City.Name,
            p.City.State, p.IsFresher, p.ExperienceYears, p.Skills, p.Interests, p.Availability, p.About, p.Area, p.ProfileImageUrl, p.ContactCount));

    private static bool ValidImageUrl(string? value) => string.IsNullOrWhiteSpace(value) ||
        (value.Length <= 2048 && Uri.TryCreate(value, UriKind.Absolute, out var uri) && uri.Scheme is "https" or "http");
    private static string? TrimOrNull(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    private static void ValidatePage(int page, int pageSize)
    {
        if (page < 1 || pageSize is < 1 or > 100 || page > int.MaxValue / pageSize)
            throw new ArgumentException("Page must be at least 1 and pageSize must be between 1 and 100.");
    }
}
