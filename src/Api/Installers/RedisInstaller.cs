using Api.Exceptions;
using Api.Options;
using StackExchange.Redis;

namespace Api.Installers;

public static class RedisInstaller
{
    public static async Task<WebApplicationBuilder> AddRedis(this WebApplicationBuilder builder)
    {
        var redisCs =
            builder.Configuration.GetConnectionString("Redis")
            ?? throw new NullConfigurationEntryException();
        var redis = await ConnectionMultiplexer.ConnectAsync(redisCs);
        builder.Services.AddSingleton(redis);
        var redisOptions = new RedisOptions();
        builder.Configuration.Bind(nameof(redisOptions), redisOptions);
        builder.Services.AddSingleton(redisOptions);

        return builder;
    }
}