namespace Api.Domain;

public class Token
{
#pragma warning disable CS8618
    public string JwtToken { get; set; }
    public string RefreshToken { get; set; }
#pragma warning restore CS8618
}