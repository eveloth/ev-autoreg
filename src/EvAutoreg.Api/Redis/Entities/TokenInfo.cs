namespace EvAutoreg.Api.Redis.Entities;

public class TokenInfo
{
#pragma warning disable CS8618
    public string Jti { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public bool Used { get; set; }
    public bool Invalidated { get; set; }
#pragma warning restore CS8618
}