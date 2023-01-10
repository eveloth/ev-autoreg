using System.Text.Json.Serialization;
using Api.Exceptions;
using Api.Extensions;
using Api.Middleware;
using DataAccessLibrary.Extensions;
using Api.Installers;
using Api.Mapping;
using Api.Redis;
using Api.Swagger;
using Api.Validators;
using FluentValidation;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Api;

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

        //app.RunMigrations();
        await app.SeedData();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
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