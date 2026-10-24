using LocalJobs.Contracts.Requests;
using LocalJobs.Contracts.Responses;

namespace LocalJobs.Application.Services;

public interface IAdService
{
    Task<List<AdCategoryResponse>> GetCategoriesAsync(CancellationToken cancellationToken = default);
    Task<PagedResponse<AdResponse>> GetAdsAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<AdResponse?> GetAdByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedResponse<AdResponse>> SearchAdsAsync(int? cityId, int? adCategoryId, string? area, string? keyword, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<AdResponse> CreateAdAsync(CreateAdRequest request, Guid userId, CancellationToken cancellationToken = default);
    Task<PagedResponse<AdResponse>> GetMyAdsAsync(Guid userId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<AdResponse?> UpdateAdAsync(Guid id, UpdateAdRequest request, Guid userId, CancellationToken cancellationToken = default);
    Task<bool> DeactivateAdAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
}
