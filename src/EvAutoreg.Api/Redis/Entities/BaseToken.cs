namespace EvAutoreg.Api.Redis.Entities;

public class BaseToken
{
#pragma warning disable CS8618
    public string TokenString { get; set; }
    public int UserId { get; set; }
#pragma warning restore CS8618
}