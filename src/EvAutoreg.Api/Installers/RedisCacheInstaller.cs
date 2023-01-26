using EvAutoreg.Api.Cache;
using EvAutoreg.Api.Options;

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

        builder.Services.AddStackExchangeRedisCache(
            options =>
                options.Configuration = builder.Configuration.GetConnectionString("RedisCache")
        );
        builder.Services.AddSingleton<IResponseCacheService, ResponseCacheService>();

        return builder;
    }
}