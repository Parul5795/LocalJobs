namespace LocalJobs.Contracts.Requests;

public record SaveWorkerProfileRequest(
    int CategoryId,
    int CityId,
    bool IsFresher,
    decimal? ExperienceYears,
    string? ExperienceDetails,
    string? Skills,
    string? Interests,
    string Availability,
    string? About);
