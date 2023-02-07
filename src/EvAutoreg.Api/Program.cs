using System.Text.Json.Serialization;
using EvAutoreg.Api.Extensions;
using EvAutoreg.Api.Installers;
using EvAutoreg.Api.Swagger;
using EvAutoreg.Api.Exceptions;
using EvAutoreg.Api.Mapping;
using EvAutoreg.Api.Middleware;
using EvAutoreg.Api.Options;
using EvAutoreg.Api.Redis;
using EvAutoreg.Api.Validators;
using EvAutoreg.Data.Extensions;
using FluentValidation;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Serilog;

namespace EvAutoreg.Api;

internal static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.InstallSerilog();
        builder.Services.AddHttpContextAccessor();

        await builder.AddRedis();
        builder.AddRedisCache();
        builder.Services.AddScoped<ITokenDb, TokenDb>();

        builder.Services
            .AddControllers(options =>
            {
                options.Conventions.Add(
                    new RouteTokenTransformerConvention(new ToSlugCaseTransformerConvention())
                );
            })
            .AddJsonOptions(options =>
            {
                var enumToStringConverter = new JsonStringEnumConverter();
                options.JsonSerializerOptions.Converters.Add(enumToStringConverter);
            });

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.AddSwagger();
        builder.Services.AddRouting(options => options.LowercaseUrls = true);
        builder.AddJwtAuthentication().AddPolicyBasedAuthorization();

        builder.Services.AddGrpcClient<Autoregistrar.AutoregistrarClient>(options =>
        {
            options.Address = new Uri(
                builder.Configuration["AutoregistrarUri"]
                    ?? throw new NullConfigurationEntryException("Autoregistrar URI wasn't set")
            );
        });

        builder
            .AddNpgsql()
            .UseAffixForModelMapping("Model")
            .AddDapperSnakeCaseConvention()
            .AddRepositories()
            .AddMigrations();

        builder.InstallServices();

        builder.Services.AddValidatorsFromAssemblyContaining<UserCredentialsValidator>();
        builder.Services.AddSingleton<IMapper, Mapper>();
        builder.Services.AddScoped<IMappingHelper, MappingHelper>();

        var app = builder.Build();

        app.ConfigureModelToDomainMapping();
        app.ConfigureDomainToModelMapping();
        app.ConfigureDomainToDtoMapping();
        app.ConfigureRequestToDomainMapping();

        app.RunMigrations();
        await app.SeedData();

        app.UseSerilogRequestLogging();

        var swaggerOptions = new SwaggerOptions();
        app.Configuration.Bind(nameof(SwaggerOptions), swaggerOptions);

        if (app.Environment.IsDevelopment() || swaggerOptions.Enabled)
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseMiddleware<ErrorHandlingMiddleware>();

        app.MapControllers();

        await app.RunAsync();
    }
}