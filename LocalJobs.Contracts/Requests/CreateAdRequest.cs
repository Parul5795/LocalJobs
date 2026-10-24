namespace LocalJobs.Contracts.Requests;

public record CreateAdRequest(
    string Title,
    string Description,
    int AdCategoryId,
    int CityId,
    decimal? Price,
    string? Area = null,
    string[]? ImageUrls = null);
