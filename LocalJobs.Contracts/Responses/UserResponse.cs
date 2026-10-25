namespace LocalJobs.Contracts.Responses;

public record UserResponse(Guid UserId, string Name, string Email, string PhoneNumber, string AccountType);
