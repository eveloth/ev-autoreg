namespace Api.Redis.Entities;

public class TokenInfo
{
    public string Jti { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public bool Used { get; set; }
    public bool Invalidated { get; set; }
}