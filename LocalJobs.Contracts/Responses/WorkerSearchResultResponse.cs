namespace LocalJobs.Contracts.Responses;

public record WorkerSearchResultResponse(
    Guid UserId,
    string Name,
    int CategoryId,
    string CategoryName,
    int CityId,
    string CityName,
    string CityState,
    bool IsFresher,
    decimal? ExperienceYears,
    string Skills,
    string Interests,
    string Availability,
    string About);
