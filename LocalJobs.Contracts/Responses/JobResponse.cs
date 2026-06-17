namespace LocalJobs.Contracts.Responses;

public record JobResponse(
    Guid Id,
    string Title,
    string Description,
    decimal? Salary,
    int CategoryId,
    string CategoryName,
    int CityId,
    string CityName,
    string CityState,
    Guid UserId,
    string UserName,
    bool IsActive,
    DateTime CreatedAt,
    DateTime UpdatedAt);
