namespace LocalJobs.Contracts.Responses;

public record ApplicationResponse(
    Guid Id,
    Guid JobId,
    string JobTitle,
    Guid UserId,
    string ApplicantName,
    string CoverLetter,
    string Status,
    DateTime CreatedAt);
