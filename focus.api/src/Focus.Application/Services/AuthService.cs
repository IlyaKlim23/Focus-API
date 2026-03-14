using Focus.Application.DTOs;
using Focus.Application.Interfaces;
using Focus.Domain.Entities;
using Focus.Domain.Interfaces;

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
            PasswordHash = passwordHasher.Hash(request.Password),
            CreatedAt = DateTime.UtcNow
        };
        await userRepository.AddAsync(user, ct);

        var (token, expiresAt) = jwtGenerator.Generate(user);
        return new AuthResponse(token, user.Id, user.Email, user.DisplayName, expiresAt);
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var user = await userRepository.GetByEmailAsync(request.Email.Trim().ToLowerInvariant(), ct);
        if (user == null) return null;
        if (!passwordHasher.Verify(request.Password, user.PasswordHash)) return null;

        var (token, expiresAt) = jwtGenerator.Generate(user);
        return new AuthResponse(token, user.Id, user.Email, user.DisplayName, expiresAt);
    }
}
