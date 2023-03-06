using EvAutoreg.Api.Exceptions;
using EvAutoreg.Api.Options;
using StackExchange.Redis;

namespace EvAutoreg.Api.Installers;

public static class RedisInstaller
{
    public static async Task<WebApplicationBuilder> AddRedis(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddOptions<RedisOptions>()
            .Bind(builder.Configuration.GetSection(RedisOptions.Redis))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        var redisOptions = builder.Configuration
            .GetRequiredSection(RedisOptions.Redis)
            .Get<RedisOptions>()!;

        var redisCs =
            builder.Configuration.GetConnectionString("Redis")
            ?? throw new NullConfigurationEntryException();

        var redis = await ConnectionMultiplexer.ConnectAsync(
            redisCs,
            options =>
            {
                options.Password = redisOptions.Password;
            }
        );
        builder.Services.AddSingleton(redis);

        return builder;
    }
}