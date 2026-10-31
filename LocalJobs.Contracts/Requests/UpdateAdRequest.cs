using System.ComponentModel.DataAnnotations;

namespace LocalJobs.Contracts.Requests;

public record UpdateAdRequest(
    string Title,
    string Description,
    int AdCategoryId,
    int CityId,
    decimal? Price,
    bool IsActive,
    string? Area = null,
    string[]? ImageUrls = null,
    [property: Required, StringLength(100, MinimumLength = 1)] string ContactPersonName = "",
    [property: Required, RegularExpression(@"^(?:\+91)?[6-9][0-9]{9}\z")] string ContactMobileNumber = "",
    bool AllowPhoneCall = false,
    bool AllowWhatsApp = false);
