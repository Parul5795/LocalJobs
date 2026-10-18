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

    public async Task<JobResponse?> CreateJobAsync(CreateJobRequest request, Guid employerId, CancellationToken cancellationToken = default)
    {
        if (employerId == Guid.Empty)
        {
            throw new ArgumentException("UserId is required.");
        }

        var employer = await _context.Users.FirstOrDefaultAsync(u => u.Id == employerId, cancellationToken);
        if (employer is null)
        {
            throw new ArgumentException($"User with ID {employerId} does not exist.");
        }
        if (employer.AccountType != LocalJobs.Domain.Enums.AccountType.Employer)
            throw new UnauthorizedAccessException("Only Employer accounts can create jobs.");

        var paymentPeriod = ParsePaymentPeriod(request.PaymentPeriod);
        ValidateJob(request.Title, request.Description, request.Salary, request.WorkType, request.ExperienceRequirement, paymentPeriod);

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
            UserId = employerId,
            WorkType = request.WorkType.Trim(),
            ExperienceRequirement = request.ExperienceRequirement.Trim(),
            PaymentPeriod = paymentPeriod,
            StartDate = request.StartDate,
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
                j.UpdatedAt, j.WorkType, j.ExperienceRequirement, j.PaymentPeriod.ToString(), j.StartDate, j.IsActive ? "Active" : "Inactive"
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
                j.UpdatedAt, j.WorkType, j.ExperienceRequirement, j.PaymentPeriod.ToString(), j.StartDate, j.IsActive ? "Active" : "Inactive"
            ))
            .ToListAsync(cancellationToken);
    }

    public async Task<List<JobResponse>> SearchJobsAsync(int? cityId, int? categoryId, string? keyword, string? workType, string? experienceRequirement, string? paymentPeriod, CancellationToken cancellationToken = default)
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

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var term = keyword.Trim().ToLower();
            query = query.Where(j => j.Title.ToLower().Contains(term) || j.Description.ToLower().Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(workType))
        {
            var term = workType.Trim().ToLower();
            query = query.Where(j => j.WorkType.ToLower().Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(experienceRequirement))
        {
            var term = experienceRequirement.Trim().ToLower();
            query = query.Where(j => j.ExperienceRequirement.ToLower().Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(paymentPeriod))
        {
            var value = ParsePaymentPeriod(paymentPeriod);
            query = query.Where(j => j.PaymentPeriod == value);
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
                j.UpdatedAt, j.WorkType, j.ExperienceRequirement, j.PaymentPeriod.ToString(), j.StartDate, j.IsActive ? "Active" : "Inactive"
            ))
            .ToListAsync(cancellationToken);
    }

    public async Task<List<JobResponse>> GetMyJobsAsync(Guid employerId, CancellationToken cancellationToken = default)
    {
        return await _context.Jobs.Where(j => j.UserId == employerId)
            .OrderByDescending(j => j.CreatedAt)
            .Select(j => new JobResponse(j.Id, j.Title, j.Description, j.Salary, j.CategoryId, j.Category.Name,
                j.CityId, j.City.Name, j.City.State, j.UserId, j.User.Name, j.IsActive, j.CreatedAt, j.UpdatedAt,
                j.WorkType, j.ExperienceRequirement, j.PaymentPeriod.ToString(), j.StartDate, j.IsActive ? "Active" : "Inactive"))
            .ToListAsync(cancellationToken);
    }

    public async Task<JobResponse?> UpdateJobAsync(Guid id, Guid employerId, UpdateJobRequest request, CancellationToken cancellationToken = default)
    {
        var job = await _context.Jobs.FirstOrDefaultAsync(j => j.Id == id, cancellationToken);
        if (job is null || job.UserId != employerId) return null;
        var paymentPeriod = ParsePaymentPeriod(request.PaymentPeriod);
        ValidateJob(request.Title, request.Description, request.Salary, request.WorkType, request.ExperienceRequirement, paymentPeriod);
        if (!await _context.Categories.AnyAsync(c => c.Id == request.CategoryId, cancellationToken) ||
            !await _context.Cities.AnyAsync(c => c.Id == request.CityId, cancellationToken))
            throw new ArgumentException("A valid category and city are required.");
        job.Title = request.Title.Trim(); job.Description = request.Description.Trim(); job.Salary = request.Salary;
        job.CategoryId = request.CategoryId; job.CityId = request.CityId; job.IsActive = request.IsActive;
        job.WorkType = request.WorkType.Trim(); job.ExperienceRequirement = request.ExperienceRequirement.Trim();
        job.PaymentPeriod = paymentPeriod; job.StartDate = request.StartDate; job.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);
        return await GetJobByIdAsync(id, cancellationToken) ?? await GetJobForOwnerAsync(id, cancellationToken);
    }

    public async Task<bool> DeactivateJobAsync(Guid id, Guid employerId, CancellationToken cancellationToken = default)
    {
        var job = await _context.Jobs.FirstOrDefaultAsync(j => j.Id == id, cancellationToken);
        if (job is null || job.UserId != employerId)
        {
            return false;
        }

        job.IsActive = false;
        job.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    private async Task<JobResponse?> GetJobForOwnerAsync(Guid id, CancellationToken cancellationToken) => await _context.Jobs
        .Where(j => j.Id == id)
        .Select(j => new JobResponse(j.Id, j.Title, j.Description, j.Salary, j.CategoryId, j.Category.Name,
            j.CityId, j.City.Name, j.City.State, j.UserId, j.User.Name, j.IsActive, j.CreatedAt, j.UpdatedAt,
            j.WorkType, j.ExperienceRequirement, j.PaymentPeriod.ToString(), j.StartDate, j.IsActive ? "Active" : "Inactive"))
        .FirstOrDefaultAsync(cancellationToken);

    private static LocalJobs.Domain.Enums.PaymentPeriod ParsePaymentPeriod(string value)
    {
        if (!Enum.TryParse<LocalJobs.Domain.Enums.PaymentPeriod>(value, true, out var period) || !Enum.IsDefined(period))
            throw new ArgumentException("PaymentPeriod must be Monthly, Daily, Hourly, or Fixed.");
        return period;
    }

    private static void ValidateJob(string title, string description, decimal? salary, string workType, string experience, LocalJobs.Domain.Enums.PaymentPeriod period)
    {
        if (string.IsNullOrWhiteSpace(title) || title.Length > 100 || string.IsNullOrWhiteSpace(description) || description.Length > 2000 ||
            (salary.HasValue && salary < 0) || string.IsNullOrWhiteSpace(workType) || workType.Length > 50 ||
            experience is null || experience.Length > 200 || !Enum.IsDefined(period))
            throw new ArgumentException("Job fields are invalid or exceed their allowed lengths.");
    }
}
