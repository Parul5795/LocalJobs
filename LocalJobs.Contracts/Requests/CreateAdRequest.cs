namespace LocalJobs.Contracts.Requests;

public record CreateAdRequest(
    string Title,
    string Description,
    int AdCategoryId,
    int CityId,
    decimal? Price);
