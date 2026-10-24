namespace LocalJobs.Contracts.Responses;

public record ContactResponse(Guid Id, Guid WorkerUserId, string Method, string PhoneNumber, int ContactCount, DateTime CreatedAt);
