using LocalJobs.Contracts.Requests;
using LocalJobs.Contracts.Responses;

namespace LocalJobs.Application.Services;

public interface IAdService
{
    Task<List<AdCategoryResponse>> GetCategoriesAsync(CancellationToken cancellationToken = default);
    Task<List<AdResponse>> GetAdsAsync(CancellationToken cancellationToken = default);
    Task<AdResponse?> GetAdByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<AdResponse>> SearchAdsAsync(int? cityId, int? adCategoryId, string? keyword, CancellationToken cancellationToken = default);
    Task<AdResponse> CreateAdAsync(CreateAdRequest request, Guid userId, CancellationToken cancellationToken = default);
    Task<List<AdResponse>> GetMyAdsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<AdResponse?> UpdateAdAsync(Guid id, UpdateAdRequest request, Guid userId, CancellationToken cancellationToken = default);
    Task<bool> DeactivateAdAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
}
