using LocalJobs.Contracts.Requests;
using LocalJobs.Contracts.Responses;

namespace LocalJobs.Application.Services;

public interface IUserService
{
    Task<AuthResponse> RegisterAsync(RegisterUserRequest request, CancellationToken cancellationToken = default);

    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
}
