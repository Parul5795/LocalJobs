using LocalJobs.Application.Interfaces;
using LocalJobs.Contracts.Requests;
using LocalJobs.Contracts.Responses;
using LocalJobs.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LocalJobs.Application.Services;

public class AdService(IApplicationDbContext context) : IAdService
{
    public Task<List<AdCategoryResponse>> GetCategoriesAsync(CancellationToken cancellationToken = default) =>
        context.AdCategories.OrderBy(c => c.Name).Select(c => new AdCategoryResponse(c.Id, c.Name)).ToListAsync(cancellationToken);

    public Task<PagedResponse<AdResponse>> GetAdsAsync(int page, int pageSize, CancellationToken cancellationToken = default) =>
        ToPageAsync(context.Ads.Where(a => a.IsActive), page, pageSize, cancellationToken);

    public Task<AdResponse?> GetAdByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        Project(context.Ads.Where(a => a.Id == id && a.IsActive), includeContactMobile: true).FirstOrDefaultAsync(cancellationToken);

    public Task<PagedResponse<AdResponse>> SearchAdsAsync(int? cityId, int? adCategoryId, string? area, string? keyword, int page, int pageSize,
        CancellationToken cancellationToken = default)
    {
        ValidatePage(page, pageSize);
        if (cityId is <= 0 || adCategoryId is <= 0 || (area?.Length ?? 0) > 100 || (keyword?.Length ?? 0) > 100)
            throw new ArgumentException("Search filters are invalid or exceed their allowed lengths.");
        var query = context.Ads.Where(a => a.IsActive);
        if (cityId.HasValue) query = query.Where(a => a.CityId == cityId.Value);
        if (adCategoryId.HasValue) query = query.Where(a => a.AdCategoryId == adCategoryId.Value);
        if (!string.IsNullOrWhiteSpace(area)) { var term = area.Trim().ToLower(); query = query.Where(a => a.Area != null && a.Area.ToLower().Contains(term)); }
        if (!string.IsNullOrWhiteSpace(keyword)) { var term = keyword.Trim().ToLower(); query = query.Where(a => a.Title.ToLower().Contains(term) || a.Description.ToLower().Contains(term)); }
        return ToPageAsync(query, page, pageSize, cancellationToken);
    }

    public async Task<AdResponse> CreateAdAsync(CreateAdRequest request, Guid userId, CancellationToken cancellationToken = default)
    {
        Validate(request.Title, request.Description, request.Price, request.Area, request.ImageUrls);
        ValidateContact(request.ContactPersonName, request.ContactMobileNumber, request.AllowPhoneCall, request.AllowWhatsApp);
        if (!await context.Users.AnyAsync(u => u.Id == userId, cancellationToken)) throw new KeyNotFoundException("User was not found.");
        await ValidateLocationAndCategory(request.AdCategoryId, request.CityId, cancellationToken);
        var ad = new Ad { Id = Guid.NewGuid(), UserId = userId, Title = request.Title.Trim(), Description = request.Description.Trim(),
            AdCategoryId = request.AdCategoryId, CityId = request.CityId, Area = TrimOrNull(request.Area), Price = request.Price,
            ImageUrls = request.ImageUrls?.Select(v => v.Trim()).ToArray() ?? [], IsActive = true,
            ContactPersonName = request.ContactPersonName.Trim(), ContactMobileNumber = request.ContactMobileNumber.Trim(),
            AllowPhoneCall = request.AllowPhoneCall, AllowWhatsApp = request.AllowWhatsApp };
        context.Ads.Add(ad);
        await context.SaveChangesAsync(cancellationToken);
        return await Project(context.Ads.Where(a => a.Id == ad.Id), includeContactMobile: true).FirstAsync(cancellationToken);
    }

    public Task<PagedResponse<AdResponse>> GetMyAdsAsync(Guid userId, int page, int pageSize, CancellationToken cancellationToken = default) =>
        ToPageAsync(context.Ads.Where(a => a.UserId == userId), page, pageSize, cancellationToken, includeContactMobile: true);

    public async Task<AdResponse?> UpdateAdAsync(Guid id, UpdateAdRequest request, Guid userId, CancellationToken cancellationToken = default)
    {
        var ad = await context.Ads.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        if (ad is null || ad.UserId != userId) return null;
        Validate(request.Title, request.Description, request.Price, request.Area, request.ImageUrls);
        ValidateContact(request.ContactPersonName, request.ContactMobileNumber, request.AllowPhoneCall, request.AllowWhatsApp);
        await ValidateLocationAndCategory(request.AdCategoryId, request.CityId, cancellationToken);
        ad.Title = request.Title.Trim(); ad.Description = request.Description.Trim(); ad.AdCategoryId = request.AdCategoryId;
        ad.CityId = request.CityId; ad.Area = TrimOrNull(request.Area); ad.Price = request.Price;
        ad.ImageUrls = request.ImageUrls?.Select(v => v.Trim()).ToArray() ?? []; ad.IsActive = request.IsActive; ad.UpdatedAt = DateTime.UtcNow;
        ad.ContactPersonName = request.ContactPersonName.Trim(); ad.ContactMobileNumber = request.ContactMobileNumber.Trim();
        ad.AllowPhoneCall = request.AllowPhoneCall; ad.AllowWhatsApp = request.AllowWhatsApp;
        await context.SaveChangesAsync(cancellationToken);
        return await Project(context.Ads.Where(a => a.Id == id), includeContactMobile: true).FirstAsync(cancellationToken);
    }

    public async Task<bool> DeactivateAdAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        var ad = await context.Ads.FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId, cancellationToken);
        if (ad is null) return false;
        ad.IsActive = false; ad.UpdatedAt = DateTime.UtcNow;
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static IQueryable<AdResponse> Project(IQueryable<Ad> query, bool includeContactMobile = false) => query.Select(a => new AdResponse(a.Id, a.Title, a.Description,
        a.AdCategoryId, a.AdCategory.Name, a.CityId, a.City.Name, a.City.State, a.Price, a.UserId, a.User.Name,
        a.IsActive, a.CreatedAt, a.UpdatedAt, a.Area, a.ImageUrls, a.ContactPersonName,
        includeContactMobile ? a.ContactMobileNumber : null, a.AllowPhoneCall, a.AllowWhatsApp));

    private async Task<PagedResponse<AdResponse>> ToPageAsync(IQueryable<Ad> query, int page, int pageSize, CancellationToken cancellationToken, bool includeContactMobile = false)
    {
        ValidatePage(page, pageSize);
        var total = await query.CountAsync(cancellationToken);
        var items = await Project(query.OrderByDescending(a => a.CreatedAt).ThenBy(a => a.Id)
            .Skip((page - 1) * pageSize).Take(pageSize), includeContactMobile).ToListAsync(cancellationToken);
        return new PagedResponse<AdResponse>(items, page, pageSize, total, (int)Math.Ceiling(total / (double)pageSize));
    }

    private async Task ValidateLocationAndCategory(int adCategoryId, int cityId, CancellationToken cancellationToken)
    {
        if (adCategoryId <= 0 || cityId <= 0 || !await context.AdCategories.AnyAsync(c => c.Id == adCategoryId, cancellationToken) ||
            !await context.Cities.AnyAsync(c => c.Id == cityId, cancellationToken)) throw new ArgumentException("A valid ad category and city are required.");
    }

    private static void Validate(string title, string description, decimal? price, string? area, string[]? images)
    {
        if (string.IsNullOrWhiteSpace(title) || title.Length > 120 || string.IsNullOrWhiteSpace(description) || description.Length > 3000 ||
            price is < 0 or > 9999999999999999m || (area?.Length ?? 0) > 100 || images is { Length: > 10 } ||
            (images?.Any(url => url.Length > 2048 || !Uri.TryCreate(url, UriKind.Absolute, out var uri) || uri.Scheme is not ("https" or "http")) ?? false))
            throw new ArgumentException("Ad fields are invalid or exceed their allowed lengths.");
    }

    private static void ValidatePage(int page, int pageSize)
    {
        if (page < 1 || pageSize is < 1 or > 100 || page > int.MaxValue / pageSize)
            throw new ArgumentException("Page must be at least 1 and pageSize must be between 1 and 100.");
    }
    private static void ValidateContact(string name, string mobile, bool allowPhoneCall, bool allowWhatsApp)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Trim().Length > 100 ||
            !System.Text.RegularExpressions.Regex.IsMatch(mobile ?? "", @"^(?:\+91)?[6-9][0-9]{9}\z") ||
            (!allowPhoneCall && !allowWhatsApp))
            throw new ArgumentException("Contact name, a valid Indian mobile number, and at least one contact method are required.");
    }
    private static string? TrimOrNull(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
