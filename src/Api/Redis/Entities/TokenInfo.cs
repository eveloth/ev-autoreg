namespace Api.Redis.Entities;

public record TokenInfo(string Jti, int UserId, DateTime CreationDate, DateTime ExpiryDate, bool Used, bool Invalidated);