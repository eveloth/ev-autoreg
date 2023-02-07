namespace EvAutoreg.Api.Options;

public class JwtOptions
{
    public string Key { get; set; } = default!;
    public string Issuer { get; set; } = default!;
    public TimeSpan Lifetime { get; set; } = default!;
    public TimeSpan RefreshTokenLifetime { get; set; }
}