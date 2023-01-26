namespace EvAutoreg.Api.Redis.Entities;

public class RefreshToken
{
#pragma warning disable CS8618
    public RefreshToken()
    {

    }
    public RefreshToken(TokenInfo tokenInfo)
    {
        Token = Guid.NewGuid().ToString();
        TokenInfo = tokenInfo;
    }
    public string Token { get; set; }
    public TokenInfo TokenInfo { get; set; }
#pragma warning restore CS8618
}