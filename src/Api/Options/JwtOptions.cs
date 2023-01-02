namespace Api.Options;

public class JwtOptions
{
#pragma warning disable CS8618
    public string Key { get; set; }
    public string Issuer { get; set; }
    public TimeSpan Lifetime { get; set; }
    public TimeSpan RefreshTokenLifetime { get; set; }
#pragma warning restore CS8618
}