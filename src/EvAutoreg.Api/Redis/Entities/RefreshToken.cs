namespace EvAutoreg.Api.Redis.Entities;

public class RefreshToken
{
    public RefreshToken()
    {

    }
    public RefreshToken(TokenInfo tokenInfo)
    {
        Token = Guid.NewGuid().ToString();
        TokenInfo = tokenInfo;
    }

    public string Token { get; set; } = default!;
    public TokenInfo TokenInfo { get; set; } = default!;
}