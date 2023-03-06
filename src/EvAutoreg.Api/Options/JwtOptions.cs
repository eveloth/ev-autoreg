using System.ComponentModel.DataAnnotations;

namespace EvAutoreg.Api.Options;

public class JwtOptions
{
    public const string Jwt = "Jwt";
    [Required]
    [MinLength(32)]
    public string Key { get; set; } = default!;
    [Required]
    public string Issuer { get; set; } = default!;
    [Required]
    public TimeSpan Lifetime { get; set; }
    [Required]
    public TimeSpan RefreshTokenLifetime { get; set; }
}