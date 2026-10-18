using LocalJobs.Contracts.Requests;
using LocalJobs.Contracts.Responses;

namespace LocalJobs.Application.Services;

public interface IWorkerProfileService
{
    Task<WorkerProfileResponse?> GetMyProfileAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<WorkerProfileResponse> SaveMyProfileAsync(Guid userId, SaveWorkerProfileRequest request, CancellationToken cancellationToken = default);
    Task<WorkerProfileResponse?> GetPublicProfileAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<List<WorkerSearchResultResponse>> SearchWorkersAsync(int? categoryId, int? cityId, bool? isFresher, string? keyword, CancellationToken cancellationToken = default);
}
