using LocalJobs.Contracts.Requests;
using LocalJobs.Contracts.Responses;

namespace LocalJobs.Application.Services;

public interface IJobService
{
    Task<JobResponse?> CreateJobAsync(CreateJobRequest request, Guid userId, CancellationToken cancellationToken = default);

    Task<JobResponse?> GetJobByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<List<JobResponse>> GetJobsAsync(CancellationToken cancellationToken = default);

    Task<List<JobResponse>> SearchJobsAsync(int? cityId, int? categoryId, CancellationToken cancellationToken = default);

    Task<bool> DeactivateJobAsync(Guid id, CancellationToken cancellationToken = default);
}
