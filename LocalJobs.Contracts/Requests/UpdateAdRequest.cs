namespace LocalJobs.Contracts.Requests;

public record UpdateAdRequest(
    string Title,
    string Description,
    int AdCategoryId,
    int CityId,
    decimal? Price,
    bool IsActive);
