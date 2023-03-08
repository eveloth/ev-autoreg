using System.ComponentModel.DataAnnotations;

namespace EvAutoreg.Api.Options;

public class RedisOptions
{
    public const string Redis = "Redis";
    [Required]
    [Range(0, 15)]
    public int RefreshTokenDb { get; set; }
    [Required]
    public string Password { get; set; } = default!;
}