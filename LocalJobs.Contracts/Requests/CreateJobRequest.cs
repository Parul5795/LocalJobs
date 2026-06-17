namespace LocalJobs.Contracts.Requests;

public record CreateJobRequest(
    string Title,
    string Description,
    decimal? Salary,
    int CategoryId,
    int CityId);
