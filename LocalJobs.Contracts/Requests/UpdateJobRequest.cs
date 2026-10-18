namespace LocalJobs.Contracts.Requests;

public record UpdateJobRequest(
    string Title,
    string Description,
    decimal? Salary,
    int CategoryId,
    int CityId,
    bool IsActive,
    string WorkType = "",
    string ExperienceRequirement = "",
    string PaymentPeriod = "Monthly",
    DateTime? StartDate = null);
