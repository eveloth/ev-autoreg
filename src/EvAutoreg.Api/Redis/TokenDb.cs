using EvAutoreg.Api.Options;
using EvAutoreg.Api.Redis.Entities;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace EvAutoreg.Api.Redis;

public class TokenDb : ITokenDb
{
    private readonly JwtOptions _jwtOptions;
    private readonly ConnectionMultiplexer _redis;
    private readonly RedisOptions _redisOptions;

    public TokenDb(ConnectionMultiplexer redis, RedisOptions redisOptions, JwtOptions jwtOptions)
    {
        _redis = redis;
        _redisOptions = redisOptions;
        _jwtOptions = jwtOptions;
    }

    public async Task SaveRefreshToken(int userId, RefreshToken token)
    {
        var db = _redis.GetDatabase(_redisOptions.RefreshTokenDb);
        await db.StringSetAsync(
            token.Token,
            JsonConvert.SerializeObject(token.TokenInfo),
            _jwtOptions.RefreshTokenLifetime
        );
        await db.StringSetAsync(
            userId.ToString(),
            JsonConvert.SerializeObject(token.Token),
            _jwtOptions.RefreshTokenLifetime
        );
    }

    public async Task<RefreshToken?> GetRefreshToken(string tokenString)
    {
        var db = _redis.GetDatabase(_redisOptions.RefreshTokenDb);
        var value = await db.StringGetAsync(tokenString);

        if (!value.HasValue)
        {
            return null;
        }

        var tokenInfo = JsonConvert.DeserializeObject<TokenInfo>(value!);

        return new RefreshToken { Token = tokenString, TokenInfo = tokenInfo! };
    }

    public async Task InvalidateRefreshToken(int userId)
    {
        var db = _redis.GetDatabase(_redisOptions.RefreshTokenDb);
        var tokenString = await db.StringGetAsync(userId.ToString());

        if (!tokenString.HasValue)
        {
            return;
        }

        var tokenRedisValue = await db.StringGetAsync(tokenString.ToString());

        if (!tokenRedisValue.HasValue)
        {
            return;
        }

        var tokenInfo = JsonConvert.DeserializeObject<TokenInfo>(tokenRedisValue!);
        tokenInfo!.Invalidated = true;

        await db.StringSetAsync(
            tokenString.ToString(),
            JsonConvert.SerializeObject(tokenInfo),
            _jwtOptions.RefreshTokenLifetime
        );
    }
}