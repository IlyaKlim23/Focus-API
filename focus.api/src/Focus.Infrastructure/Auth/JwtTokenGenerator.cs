using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Focus.Application.Interfaces;
using Focus.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Focus.Infrastructure.Auth;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtSettings _settings;

    public JwtTokenGenerator(IOptions<JwtSettings> options)
    {
        _settings = options.Value;
    }

    public (string Token, DateTime ExpiresAt) Generate(User user)
    {
        var expiration = _settings.ExpirationDays > 0
            ? TimeSpan.FromDays(_settings.ExpirationDays)
            : _settings.Expiration;
        var expiresAt = DateTime.UtcNow.Add(expiration);
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("userId", user.Id.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: creds);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        return (tokenString, expiresAt);
    }
}

/// <summary>
/// Настройки JWT
/// </summary>
public class JwtSettings
{
    /// <summary>
    /// Имя секции в конфигурации
    /// </summary>
    public const string Section = "Jwt";

    /// <summary>
    /// Секретный ключ для подписи токена
    /// </summary>
    public string SecretKey { get; set; } = string.Empty;

    /// <summary>
    /// Издатель токена
    /// </summary>
    public string Issuer { get; set; } = "FocusApi";

    /// <summary>
    /// Аудитория токена
    /// </summary>
    public string Audience { get; set; } = "FocusApi";

    /// <summary>
    /// Срок действия токена в днях
    /// </summary>
    public int ExpirationDays { get; set; } = 7;

    /// <summary>
    /// Срок действия токена
    /// </summary>
    public TimeSpan Expiration { get; set; } = TimeSpan.FromDays(7);
}
