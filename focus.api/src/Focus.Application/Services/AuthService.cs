using Focus.Application.DTOs;
using Focus.Application.Interfaces;
using Focus.Domain.Entities;
using Focus.Domain.Interfaces;
using Focus.Domain.ValueObjects;

namespace Focus.Application.Services;

public class AuthService(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IJwtTokenGenerator jwtGenerator) : IAuthService
{
    public async Task<AuthResponse?> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        var existing = await userRepository.GetByEmailAsync(request.Email, ct);
        if (existing != null) return null;

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email.Trim().ToLowerInvariant(),
            DisplayName = request.DisplayName?.Trim(),
            Role = UserRole.User,
            PasswordHash = passwordHasher.Hash(request.Password),
            CreatedAt = DateTime.UtcNow
        };
        await userRepository.AddAsync(user, ct);

        var (token, expiresAt) = jwtGenerator.Generate(user);
        return new AuthResponse(token, user.Id, user.Email, user.DisplayName, user.Role, expiresAt);
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var user = await userRepository.GetByEmailAsync(request.Email.Trim().ToLowerInvariant(), ct);
        if (user == null) return null;
        if (!passwordHasher.Verify(request.Password, user.PasswordHash)) return null;

        var (token, expiresAt) = jwtGenerator.Generate(user);
        return new AuthResponse(token, user.Id, user.Email, user.DisplayName, user.Role, expiresAt);
    }

    public async Task<bool> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken ct = default)
    {
        var user = await userRepository.GetByEmailAsync(request.Email.Trim().ToLowerInvariant(), ct);
        if (user == null) return false;
        user.PasswordHash = passwordHasher.Hash(request.NewPassword);
        await userRepository.UpdateAsync(user, ct);
        return true;
    }
}
