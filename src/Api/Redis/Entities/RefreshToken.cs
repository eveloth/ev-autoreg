namespace Api.Redis.Entities;

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
    public string Token { get; set; }
    public TokenInfo TokenInfo { get; set; }
}