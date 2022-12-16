using Api.Redis.Entities;

namespace Api.Redis;

public interface ITokenDb
{
    Task SaveRefreshToken(RefreshToken token);
    Task<RefreshToken?> GetRefreshToken(string key);
}