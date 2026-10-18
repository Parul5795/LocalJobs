namespace LocalJobs.Contracts.Responses;

public record WorkerProfileResponse(
    Guid UserId,
    string Name,
    int CategoryId,
    string CategoryName,
    int CityId,
    string CityName,
    string CityState,
    bool IsFresher,
    decimal? ExperienceYears,
    string ExperienceDetails,
    string Skills,
    string Interests,
    string Availability,
    string About,
    DateTime CreatedAt,
    DateTime UpdatedAt);
