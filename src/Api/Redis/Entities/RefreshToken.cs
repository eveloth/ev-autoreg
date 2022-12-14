namespace Api.Redis.Entities;

public record RefreshToken
{
    public RefreshToken()
    {

    }
    public RefreshToken(TokenInfo tokenInfo)
    {
        Token = Guid.NewGuid().ToString();
        TokenInfo = tokenInfo;
    }
    public string? Token { get; init; }
    public TokenInfo? TokenInfo { get; init; }
}