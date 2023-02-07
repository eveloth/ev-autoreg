using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace EvAutoreg.Api.Cache;

public class ResponseCacheService : IResponseCacheService
{
    private JsonSerializerOptions _serializerOptions =
        new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
    private readonly IDistributedCache _distributedCache;

    public ResponseCacheService(IDistributedCache distributedCache)
    {
        _distributedCache = distributedCache;
    }

    public async Task CacheResponse(string cacheKey, object response, TimeSpan ttl)
    {
        var serializedResponse = JsonSerializer.Serialize(response, _serializerOptions);
        await _distributedCache.SetStringAsync(
            cacheKey,
            serializedResponse,
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = ttl }
        );
    }

    public async Task<string?> GetCachedResponse(string cacheKey)
    {
        var cachedResponse = await _distributedCache.GetStringAsync(cacheKey);

        return string.IsNullOrEmpty(cachedResponse) ? null : cachedResponse;
    }
}