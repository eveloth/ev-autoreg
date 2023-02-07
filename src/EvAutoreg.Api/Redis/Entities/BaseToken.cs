namespace EvAutoreg.Api.Redis.Entities;

public class BaseToken
{
    public string TokenString { get; set; } = default!;
    public int UserId { get; set; }
}