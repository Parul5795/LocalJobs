namespace LocalJobs.Contracts.Responses;

public record AdResponse(
    Guid Id,
    string Title,
    string Description,
    int AdCategoryId,
    string AdCategoryName,
    int CityId,
    string CityName,
    string CityState,
    decimal? Price,
    Guid UserId,
    string UserName,
    bool IsActive,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    string? Area = null,
    string[]? ImageUrls = null);
