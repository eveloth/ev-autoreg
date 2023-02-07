using EvAutoreg.Api.Cache;
using EvAutoreg.Api.Options;
using StackExchange.Redis;

namespace EvAutoreg.Api.Installers;

public static class RedisCacheInstaller
{
    public static WebApplicationBuilder AddRedisCache(this WebApplicationBuilder builder)
    {
        var redisCacheOptions = new RedisCacheOptions();
        builder.Configuration.Bind(nameof(redisCacheOptions), redisCacheOptions);
        builder.Services.AddSingleton(redisCacheOptions);

        if (!redisCacheOptions.Enabled)
            return builder;

        var redisOptions = new RedisOptions();
        builder.Configuration.Bind(nameof(redisOptions), redisOptions);

        var redisConnectionString = builder.Configuration.GetConnectionString("RedisCache");
        var config = ConfigurationOptions.Parse(redisConnectionString!);
        config.Password = redisOptions.Password;

        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.ConfigurationOptions = config;
        });

        builder.Services.AddSingleton<IResponseCacheService, ResponseCacheService>();

        return builder;
    }
}