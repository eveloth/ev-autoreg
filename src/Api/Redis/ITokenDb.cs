using Api.Redis.Entities;

namespace Api.Redis;

public interface ITokenDb
{
    Task SaveRefreshToken(int userId, RefreshToken token);
    Task<RefreshToken?> GetRefreshToken(string tokenString);
    Task InvalidateRefreshToken(int userId);
}