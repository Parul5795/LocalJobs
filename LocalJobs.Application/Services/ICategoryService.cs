using LocalJobs.Contracts.Responses;

namespace LocalJobs.Application.Services;

public interface ICategoryService
{
    Task<List<CategoryResponse>> GetCategoriesAsync(CancellationToken cancellationToken = default);
}
