namespace LocalJobs.Contracts.Responses;

public record AuthResponse(
    string Token,
    Guid UserId,
    string Name,
    string Email);
