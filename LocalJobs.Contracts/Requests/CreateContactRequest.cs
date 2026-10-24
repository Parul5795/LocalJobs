namespace LocalJobs.Contracts.Requests;

public record CreateContactRequest(Guid WorkerUserId, string Method);
