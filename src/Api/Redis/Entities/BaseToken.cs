namespace Api.Redis.Entities;

public class BaseToken
{
    public string TokenString { get; set; }
    public int UserId { get; set; }
}