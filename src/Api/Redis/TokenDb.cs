using Api.Options;
using Api.Redis.Entities;
using Newtonsoft.Json;
using NuGet.Protocol;
using StackExchange.Redis;

namespace Api.Redis;

public class TokenDb : ITokenDb
{
    private readonly ConnectionMultiplexer _redis;
    private readonly RedisOptions _redisOptions;

    public TokenDb(ConnectionMultiplexer redis, RedisOptions redisOptions)
    {
        _redis = redis;
        _redisOptions = redisOptions;
    }

    public async Task SaveRefreshToken(RefreshToken token)
    {
        var db = _redis.GetDatabase(_redisOptions.RefreshTokenDb);
        await db.StringSetAsync(token.Token, token.TokenInfo.ToJson());
    }

    public async Task<RefreshToken?> GetRefreshToken(string key)
    {
        var db = _redis.GetDatabase(_redisOptions.RefreshTokenDb);
        var value = await db.StringGetAsync(key);
        var tokenInfo = JsonConvert.DeserializeObject<TokenInfo>(value.ToJson());

        return new RefreshToken
        {
            Token = key,
            TokenInfo = tokenInfo
        };
    }

    public async Task<string?> TestRedis(string key, string value)
    {
        var db = _redis.GetDatabase(_redisOptions.RefreshTokenDb);
        db.StringSet(key, value);

        var result = await db.StringGetAsync(key);
        Console.WriteLine(result);
        return result;
    }
}