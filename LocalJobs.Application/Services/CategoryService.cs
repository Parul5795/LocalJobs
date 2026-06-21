using LocalJobs.Application.Interfaces;
using LocalJobs.Contracts.Responses;
using Microsoft.EntityFrameworkCore;

namespace LocalJobs.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly IApplicationDbContext _context;

    public CategoryService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<CategoryResponse>> GetCategoriesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Categories
            .OrderBy(c => c.Name)
            .Select(c => new CategoryResponse(c.Id, c.Name))
            .ToListAsync(cancellationToken);
    }
}
