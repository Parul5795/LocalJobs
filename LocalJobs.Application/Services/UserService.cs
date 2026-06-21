using BCrypt.Net;
using LocalJobs.Application.Interfaces;
using LocalJobs.Contracts.Requests;
using LocalJobs.Contracts.Responses;
using LocalJobs.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LocalJobs.Application.Services;

public class UserService : IUserService
{
    private readonly IApplicationDbContext _context;
    private readonly IJwtTokenService _jwtTokenService;

    public UserService(IApplicationDbContext context, IJwtTokenService jwtTokenService)
    {
        _context = context;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterUserRequest request, CancellationToken cancellationToken = default)
    {
        var email = request.Email.Trim();
        var normalizedEmail = email.ToLowerInvariant();

        var emailExists = await _context.Users
            .AnyAsync(u => u.Email.ToLower() == normalizedEmail, cancellationToken);

        if (emailExists)
        {
            throw new ArgumentException("A user with this email already exists.");
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = new User
        {
            Name = request.Name.Trim(),
            Email = email,
            Phone = request.PhoneNumber.Trim(),
            PasswordHash = passwordHash
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        var token = _jwtTokenService.GenerateToken(user);

        return new AuthResponse(token, user.Id, user.Name, user.Email);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var email = request.Email.Trim();

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLowerInvariant(), cancellationToken);

        if (user is null)
        {
            throw new ArgumentException("Invalid email or password.");
        }

        var passwordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
        if (!passwordValid)
        {
            throw new ArgumentException("Invalid email or password.");
        }

        var token = _jwtTokenService.GenerateToken(user);
        return new AuthResponse(token, user.Id, user.Name, user.Email);
    }
}
