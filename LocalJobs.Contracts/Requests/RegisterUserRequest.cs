namespace LocalJobs.Contracts.Requests;

public record RegisterUserRequest(
    string Name,
    string Email,
    string PhoneNumber,
    string Password);
