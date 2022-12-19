namespace Api.Cache;

public interface IResponseCacheService
{
    Task CacheResponse(string cacheKey, object response, TimeSpan ttl);
    Task<string?> GetCachedResponse(string cacheKey);
}