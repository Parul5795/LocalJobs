using LocalJobs.Application.Interfaces;
using LocalJobs.Contracts.Requests;
using LocalJobs.Contracts.Responses;
using LocalJobs.Domain.Entities;
using LocalJobs.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace LocalJobs.Application.Services;

public class JobService(IApplicationDbContext context) : IJobService
{
    public async Task<JobResponse?> CreateJobAsync(CreateJobRequest request, Guid employerId, CancellationToken cancellationToken = default)
    {
        if (employerId == Guid.Empty) throw new ArgumentException("A valid authenticated user is required.");
        var employer = await context.Users.FirstOrDefaultAsync(u => u.Id == employerId, cancellationToken);
        if (employer is null) throw new KeyNotFoundException("Employer was not found.");
        if (employer.AccountType != AccountType.Employer) throw new UnauthorizedAccessException("Only Employer accounts can create jobs.");
        var paymentPeriod = ParsePaymentPeriod(request.PaymentPeriod);
        ValidateJob(request.Title, request.Description, request.Salary, request.WorkType, request.ExperienceRequirement, request.StartDate, request.Area);
        await ValidateCategoryAndCity(request.CategoryId, request.CityId, cancellationToken);

        var job = new Job
        {
            Id = Guid.NewGuid(), Title = request.Title.Trim(), Description = request.Description.Trim(), Salary = request.Salary,
            CategoryId = request.CategoryId, CityId = request.CityId, Area = TrimOrNull(request.Area), UserId = employerId,
            WorkType = request.WorkType.Trim(), ExperienceRequirement = request.ExperienceRequirement.Trim(),
            PaymentPeriod = paymentPeriod, StartDate = NormalizeUtc(request.StartDate), IsActive = true
        };
        context.Jobs.Add(job);
        await context.SaveChangesAsync(cancellationToken);
        return await GetJobByIdAsync(job.Id, cancellationToken);
    }

    public Task<JobResponse?> GetJobByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        Project(context.Jobs.Where(j => j.Id == id && j.IsActive)).FirstOrDefaultAsync(cancellationToken);

    public Task<PagedResponse<JobResponse>> GetJobsAsync(int page, int pageSize, CancellationToken cancellationToken = default) =>
        ToPageAsync(context.Jobs.Where(j => j.IsActive), page, pageSize, cancellationToken);

    public Task<PagedResponse<JobResponse>> SearchJobsAsync(int? cityId, int? categoryId, string? area, string? keyword,
        string? workType, string? experienceRequirement, string? paymentPeriod, int page, int pageSize,
        CancellationToken cancellationToken = default)
    {
        ValidatePage(page, pageSize);
        ValidateFilterIds(cityId, categoryId);
        ValidateSearchText(area, keyword, workType, experienceRequirement);
        var query = context.Jobs.Where(j => j.IsActive);
        if (cityId.HasValue) query = query.Where(j => j.CityId == cityId.Value);
        if (categoryId.HasValue) query = query.Where(j => j.CategoryId == categoryId.Value);
        if (!string.IsNullOrWhiteSpace(area)) { var value = area.Trim().ToLower(); query = query.Where(j => j.Area != null && j.Area.ToLower().Contains(value)); }
        if (!string.IsNullOrWhiteSpace(keyword)) { var value = keyword.Trim().ToLower(); query = query.Where(j => j.Title.ToLower().Contains(value) || j.Description.ToLower().Contains(value)); }
        if (!string.IsNullOrWhiteSpace(workType)) { var value = workType.Trim().ToLower(); query = query.Where(j => j.WorkType.ToLower().Contains(value)); }
        if (!string.IsNullOrWhiteSpace(experienceRequirement)) { var value = experienceRequirement.Trim().ToLower(); query = query.Where(j => j.ExperienceRequirement.ToLower().Contains(value)); }
        if (!string.IsNullOrWhiteSpace(paymentPeriod))
        {
            var period = ParsePaymentPeriod(paymentPeriod);
            query = query.Where(j => j.PaymentPeriod == period);
        }
        return ToPageAsync(query, page, pageSize, cancellationToken);
    }

    public Task<PagedResponse<JobResponse>> GetMyJobsAsync(Guid employerId, int page, int pageSize, CancellationToken cancellationToken = default) =>
        ToPageAsync(context.Jobs.Where(j => j.UserId == employerId), page, pageSize, cancellationToken);

    public async Task<JobResponse?> UpdateJobAsync(Guid id, Guid employerId, UpdateJobRequest request, CancellationToken cancellationToken = default)
    {
        var job = await context.Jobs.FirstOrDefaultAsync(j => j.Id == id, cancellationToken);
        if (job is null || job.UserId != employerId) return null;
        var paymentPeriod = ParsePaymentPeriod(request.PaymentPeriod);
        ValidateJob(request.Title, request.Description, request.Salary, request.WorkType, request.ExperienceRequirement, request.StartDate, request.Area);
        await ValidateCategoryAndCity(request.CategoryId, request.CityId, cancellationToken);
        job.Title = request.Title.Trim(); job.Description = request.Description.Trim(); job.Salary = request.Salary;
        job.CategoryId = request.CategoryId; job.CityId = request.CityId; job.Area = TrimOrNull(request.Area); job.IsActive = request.IsActive;
        job.WorkType = request.WorkType.Trim(); job.ExperienceRequirement = request.ExperienceRequirement.Trim();
        job.PaymentPeriod = paymentPeriod; job.StartDate = NormalizeUtc(request.StartDate); job.UpdatedAt = DateTime.UtcNow;
        await context.SaveChangesAsync(cancellationToken);
        return await Project(context.Jobs.Where(j => j.Id == id)).FirstAsync(cancellationToken);
    }

    public async Task<bool> DeactivateJobAsync(Guid id, Guid employerId, CancellationToken cancellationToken = default)
    {
        var job = await context.Jobs.FirstOrDefaultAsync(j => j.Id == id && j.UserId == employerId, cancellationToken);
        if (job is null) return false;
        job.IsActive = false; job.UpdatedAt = DateTime.UtcNow;
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static IQueryable<JobResponse> Project(IQueryable<Job> query) => query.Select(j => new JobResponse(
        j.Id, j.Title, j.Description, j.Salary, j.CategoryId, j.Category.Name, j.CityId, j.City.Name, j.City.State,
        j.UserId, j.User.Name, j.IsActive, j.CreatedAt, j.UpdatedAt, j.WorkType, j.ExperienceRequirement,
        j.PaymentPeriod.ToString(), j.StartDate, j.IsActive ? "Active" : "Inactive", j.Area));

    private async Task<PagedResponse<JobResponse>> ToPageAsync(IQueryable<Job> query, int page, int pageSize, CancellationToken cancellationToken)
    {
        ValidatePage(page, pageSize);
        var total = await query.CountAsync(cancellationToken);
        var items = await Project(query.OrderByDescending(j => j.CreatedAt).ThenBy(j => j.Id)
            .Skip((page - 1) * pageSize).Take(pageSize)).ToListAsync(cancellationToken);
        return new PagedResponse<JobResponse>(items, page, pageSize, total, (int)Math.Ceiling(total / (double)pageSize));
    }

    private async Task ValidateCategoryAndCity(int categoryId, int cityId, CancellationToken cancellationToken)
    {
        if (categoryId <= 0 || cityId <= 0 || !await context.Categories.AnyAsync(c => c.Id == categoryId, cancellationToken) ||
            !await context.Cities.AnyAsync(c => c.Id == cityId, cancellationToken))
            throw new ArgumentException("A valid category and city are required.");
    }

    private static PaymentPeriod ParsePaymentPeriod(string value) =>
        Enum.TryParse<PaymentPeriod>(value, true, out var period) && Enum.IsDefined(period)
            ? period : throw new ArgumentException("PaymentPeriod must be Monthly, Daily, Hourly, or Fixed.");

    private static void ValidateJob(string title, string description, decimal? salary, string workType, string experience, DateTime? startDate, string? area)
    {
        if (string.IsNullOrWhiteSpace(title) || title.Length > 100 || string.IsNullOrWhiteSpace(description) || description.Length > 2000 ||
            salary is < 0 or > 9999999999999999m || string.IsNullOrWhiteSpace(workType) || workType.Length > 50 ||
            string.IsNullOrWhiteSpace(experience) || experience.Length > 200 || (area?.Length ?? 0) > 100 ||
            (startDate.HasValue && startDate.Value.Date < DateTime.UtcNow.Date))
            throw new ArgumentException("Job fields are invalid or exceed their allowed lengths.");
    }

    private static void ValidatePage(int page, int pageSize)
    {
        if (page < 1 || pageSize is < 1 or > 100 || page > int.MaxValue / pageSize)
            throw new ArgumentException("Page must be at least 1 and pageSize must be between 1 and 100.");
    }

    private static void ValidateFilterIds(int? cityId, int? categoryId)
    {
        if (cityId is <= 0 || categoryId is <= 0) throw new ArgumentException("Filter IDs must be positive.");
    }

    private static void ValidateSearchText(params string?[] values)
    {
        if (values.Any(v => v?.Length > 100)) throw new ArgumentException("Search filters cannot exceed 100 characters.");
    }

    private static string? TrimOrNull(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    private static DateTime? NormalizeUtc(DateTime? value) => value.HasValue
        ? value.Value.Kind == DateTimeKind.Local ? value.Value.ToUniversalTime() : DateTime.SpecifyKind(value.Value, DateTimeKind.Utc)
        : null;
}
