using System.ComponentModel.DataAnnotations;

namespace Messenger.Application.Common.Models;

public class AppSettings
{
    [Required] public string Name { get; set; } = null!;
    [Required] public JwtSettings JwtSettings { get; set; } = null!;
}

public class JwtSettings
{
    [Required] public string SecretKey { get; set; } = null!;

    [Required] public string Issuer { get; set; } = null!;

    [Required] public string Audience { get; set; } = null!;

    [Required] public int CodeLength { get; set; }

    [Required] public int MaxAttempts { get; set; }

    [Required] public int ExpirationMinutes { get; set; }

    [Required] public int CooldownSeconds { get; init; }

    [Required] public int RefreshTokenDays { get; init; }
}
