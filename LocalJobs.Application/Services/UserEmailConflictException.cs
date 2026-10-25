namespace LocalJobs.Application.Services;

public sealed class UserEmailConflictException : Exception
{
    public UserEmailConflictException() : base("A user with this email already exists.") { }
}
