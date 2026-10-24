using LocalJobs.Contracts.Requests;
using LocalJobs.Contracts.Responses;

namespace LocalJobs.Application.Services;

public interface IWorkerProfileService
{
    Task<WorkerProfileResponse?> GetMyProfileAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<WorkerProfileResponse> SaveMyProfileAsync(Guid userId, SaveWorkerProfileRequest request, CancellationToken cancellationToken = default);
    Task<WorkerProfileResponse?> GetPublicProfileAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<PagedResponse<WorkerSearchResultResponse>> SearchWorkersAsync(int? categoryId, int? cityId, string? area, bool? isFresher, string? keyword, int page, int pageSize, CancellationToken cancellationToken = default);
}
