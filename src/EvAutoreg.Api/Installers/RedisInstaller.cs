﻿using EvAutoreg.Api.Exceptions;
using EvAutoreg.Api.Options;
using StackExchange.Redis;

namespace EvAutoreg.Api.Installers;

public static class RedisInstaller
{
    public static async Task<WebApplicationBuilder> AddRedis(this WebApplicationBuilder builder)
    {
        var redisCs =
            builder.Configuration.GetConnectionString("Redis")
            ?? throw new NullConfigurationEntryException();

        var redisOptions = new RedisOptions();
        builder.Configuration.Bind(nameof(redisOptions), redisOptions);
        builder.Services.AddSingleton(redisOptions);

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