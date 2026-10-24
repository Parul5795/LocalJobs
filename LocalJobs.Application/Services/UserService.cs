using BCrypt.Net;
using LocalJobs.Application.Interfaces;
using LocalJobs.Contracts.Requests;
using LocalJobs.Contracts.Responses;
using LocalJobs.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;

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
        if (string.IsNullOrWhiteSpace(request.Name) || request.Name.Trim().Length > 100 || string.IsNullOrWhiteSpace(request.Email) ||
            request.Email.Trim().Length > 150 || !MailAddress.TryCreate(request.Email.Trim(), out _) ||
            string.IsNullOrWhiteSpace(request.PhoneNumber) || request.PhoneNumber.Trim().Length > 20 ||
            string.IsNullOrWhiteSpace(request.Password) || request.Password.Length is < 8 or > 128 ||
            !Enum.TryParse<LocalJobs.Domain.Enums.AccountType>(request.AccountType, true, out var accountType) ||
            !Enum.IsDefined(accountType))
        {
            throw new ArgumentException("Name, email, phone, password, and a valid account type are required.");
        }

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
            PasswordHash = passwordHash,
            AccountType = accountType
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        var token = _jwtTokenService.GenerateToken(user);

        return new AuthResponse(token, user.Id, user.Name, user.Email, user.AccountType.ToString());
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || request.Email.Length > 150 || string.IsNullOrWhiteSpace(request.Password) || request.Password.Length > 128)
            throw new ArgumentException("Invalid email or password.");
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
        return new AuthResponse(token, user.Id, user.Name, user.Email, user.AccountType.ToString());
    }
}
