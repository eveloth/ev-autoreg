using EvAutoreg.Api.Cache;
using EvAutoreg.Api.Options;
using StackExchange.Redis;

namespace EvAutoreg.Api.Installers;

public static class RedisCacheInstaller
{
    public static WebApplicationBuilder AddRedisCache(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddOptions<RedisCacheOptions>()
            .Bind(builder.Configuration.GetSection(RedisCacheOptions.RedisCache))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        var redisCacheOptions = builder.Configuration
            .GetRequiredSection(RedisCacheOptions.RedisCache)
            .Get<RedisCacheOptions>()!;

        if (!redisCacheOptions.Enabled)
        {
            return builder;
        }

        var redisOptions = builder.Configuration
            .GetRequiredSection(RedisOptions.Redis)
            .Get<RedisOptions>()!;

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