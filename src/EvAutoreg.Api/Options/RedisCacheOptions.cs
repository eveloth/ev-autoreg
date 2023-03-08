using System.ComponentModel.DataAnnotations;

namespace EvAutoreg.Api.Options;

public class RedisCacheOptions
{
    public const string RedisCache = "RedisCache";
    [Required]
    public bool Enabled { get; set; }
}