using LocalJobs.Domain.Entities;

namespace LocalJobs.Application.Interfaces;

public interface IJwtTokenService
{
    string GenerateToken(User user);
}
