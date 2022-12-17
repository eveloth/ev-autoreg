namespace Api.Options;

public class JwtOptions
{
    public string Key { get; set; }
    public string Issuer { get; set; }
    public TimeSpan Lifetime { get; set; }
    public TimeSpan RefreshTokenLifetime { get; set; }
}