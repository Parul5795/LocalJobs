using LocalJobs.Application.Interfaces;
using LocalJobs.Contracts.Requests;
using LocalJobs.Contracts.Responses;
using LocalJobs.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LocalJobs.Application.Services;

public class AdService : IAdService
{
    private readonly IApplicationDbContext _context;
    public AdService(IApplicationDbContext context) => _context = context;

    public Task<List<AdCategoryResponse>> GetCategoriesAsync(CancellationToken cancellationToken = default) =>
        _context.AdCategories.OrderBy(c => c.Name).Select(c => new AdCategoryResponse(c.Id, c.Name)).ToListAsync(cancellationToken);

    public Task<List<AdResponse>> GetAdsAsync(CancellationToken cancellationToken = default) =>
        AdQuery().Where(a => a.IsActive).OrderByDescending(a => a.CreatedAt).ToListAsync(cancellationToken);

    public Task<AdResponse?> GetAdByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        AdQuery().FirstOrDefaultAsync(a => a.Id == id && a.IsActive, cancellationToken);

    public Task<List<AdResponse>> SearchAdsAsync(int? cityId, int? adCategoryId, string? keyword, CancellationToken cancellationToken = default)
    {
        var query = AdQuery().Where(a => a.IsActive);
        if (cityId.HasValue) query = query.Where(a => a.CityId == cityId.Value);
        if (adCategoryId.HasValue) query = query.Where(a => a.AdCategoryId == adCategoryId.Value);
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var term = keyword.Trim().ToLower();
            query = query.Where(a => a.Title.ToLower().Contains(term) || a.Description.ToLower().Contains(term));
        }
        return query.OrderByDescending(a => a.CreatedAt).ToListAsync(cancellationToken);
    }

    public async Task<AdResponse> CreateAdAsync(CreateAdRequest request, Guid userId, CancellationToken cancellationToken = default)
    {
        Validate(request.Title, request.Description, request.Price);
        if (!await _context.Users.AnyAsync(u => u.Id == userId, cancellationToken))
            throw new KeyNotFoundException("User was not found.");
        await ValidateLocationAndCategory(request.AdCategoryId, request.CityId, cancellationToken);

        var ad = new Ad
        {
            Id = Guid.NewGuid(), UserId = userId, Title = request.Title.Trim(), Description = request.Description.Trim(),
            AdCategoryId = request.AdCategoryId, CityId = request.CityId, Price = request.Price, IsActive = true
        };
        _context.Ads.Add(ad);
        await _context.SaveChangesAsync(cancellationToken);
        return (await AdQuery().FirstAsync(a => a.Id == ad.Id, cancellationToken));
    }

    public Task<List<AdResponse>> GetMyAdsAsync(Guid userId, CancellationToken cancellationToken = default) =>
        AdQuery().Where(a => a.UserId == userId).OrderByDescending(a => a.CreatedAt).ToListAsync(cancellationToken);

    public async Task<AdResponse?> UpdateAdAsync(Guid id, UpdateAdRequest request, Guid userId, CancellationToken cancellationToken = default)
    {
        var ad = await _context.Ads.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        if (ad is null || ad.UserId != userId) return null;
        Validate(request.Title, request.Description, request.Price);
        await ValidateLocationAndCategory(request.AdCategoryId, request.CityId, cancellationToken);
        ad.Title = request.Title.Trim(); ad.Description = request.Description.Trim();
        ad.AdCategoryId = request.AdCategoryId; ad.CityId = request.CityId; ad.Price = request.Price;
        ad.IsActive = request.IsActive; ad.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);
        return await AdQuery().FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<bool> DeactivateAdAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        var ad = await _context.Ads.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        if (ad is null || ad.UserId != userId) return false;
        ad.IsActive = false; ad.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    private IQueryable<AdResponse> AdQuery() => _context.Ads.Select(a => new AdResponse(
        a.Id, a.Title, a.Description, a.AdCategoryId, a.AdCategory.Name, a.CityId, a.City.Name, a.City.State,
        a.Price, a.UserId, a.User.Name, a.IsActive, a.CreatedAt, a.UpdatedAt));

    private async Task ValidateLocationAndCategory(int adCategoryId, int cityId, CancellationToken cancellationToken)
    {
        if (!await _context.AdCategories.AnyAsync(c => c.Id == adCategoryId, cancellationToken))
            throw new ArgumentException("A valid ad category is required.");
        if (!await _context.Cities.AnyAsync(c => c.Id == cityId, cancellationToken))
            throw new ArgumentException("A valid city is required.");
    }

    private static void Validate(string title, string description, decimal? price)
    {
        if (string.IsNullOrWhiteSpace(title) || title.Length > 120 || string.IsNullOrWhiteSpace(description) ||
            description.Length > 3000 || (price.HasValue && price.Value < 0))
            throw new ArgumentException("Ad fields are invalid or exceed their allowed lengths.");
    }
}
