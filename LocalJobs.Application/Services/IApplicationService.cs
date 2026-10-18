using LocalJobs.Contracts.Responses;
using LocalJobs.Contracts.Requests;

namespace LocalJobs.Application.Services;

public interface IApplicationService
{
    Task<ApplicationResponse> ApplyForJobAsync(Guid jobId, Guid userId, ApplyForJobRequest request, CancellationToken cancellationToken = default);

    Task<List<ApplicationResponse>> GetMyApplicationsAsync(Guid userId, CancellationToken cancellationToken = default);
}
