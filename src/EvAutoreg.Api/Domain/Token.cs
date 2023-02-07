namespace EvAutoreg.Api.Domain;

public class Token
{
    public string JwtToken { get; set; } = default!;
    public string RefreshToken { get; set; } = default!;
}