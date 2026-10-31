using System.ComponentModel.DataAnnotations;

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
    DateTime? StartDate = null,
    string? Area = null,
    [property: Required, StringLength(100, MinimumLength = 1)] string ContactPersonName = "",
    [property: Required, RegularExpression(@"^(?:\+91)?[6-9][0-9]{9}\z")] string ContactMobileNumber = "",
    bool AllowPhoneCall = false,
    bool AllowWhatsApp = false);
