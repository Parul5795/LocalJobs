using LocalJobs.Contracts.Requests;
using LocalJobs.Contracts.Responses;

namespace LocalJobs.Application.Services;

public interface IJobService
{
    Task<JobResponse?> CreateJobAsync(CreateJobRequest request, Guid employerId, CancellationToken cancellationToken = default);

    Task<JobResponse?> GetJobByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<PagedResponse<JobResponse>> GetJobsAsync(int page, int pageSize, CancellationToken cancellationToken = default);

    Task<PagedResponse<JobResponse>> SearchJobsAsync(int? cityId, int? categoryId, string? area, string? keyword, string? workType, string? experienceRequirement, string? paymentPeriod, int page, int pageSize, CancellationToken cancellationToken = default);

    Task<PagedResponse<JobResponse>> GetMyJobsAsync(Guid employerId, int page, int pageSize, CancellationToken cancellationToken = default);

    Task<JobResponse?> UpdateJobAsync(Guid id, Guid employerId, UpdateJobRequest request, CancellationToken cancellationToken = default);

    Task<bool> DeactivateJobAsync(Guid id, Guid employerId, CancellationToken cancellationToken = default);
}
