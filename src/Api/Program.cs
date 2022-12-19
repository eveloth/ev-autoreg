using System.Text.Json;
using Api.Cache;
using Api.Exceptions;
using Api.Middleware;
using Api.Services;
using Api.Services.Interfaces;
using DataAccessLibrary.Extensions;
using Api.Extensions;
using Api.Mapping;
using Api.Options;
using Api.Redis;
using Api.Validators;
using FluentMigrator.Runner;
using FluentValidation;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Serilog;
using StackExchange.Redis;

namespace Api;

internal static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        #region Redis

        var redisCs =
            builder.Configuration.GetConnectionString("Redis")
            ?? throw new NullConfigurationEntryException();
        var redis = await ConnectionMultiplexer.ConnectAsync(redisCs);
        builder.Services.AddSingleton(redis);
        var redisOptions = new RedisOptions();
        builder.Configuration.Bind(nameof(redisOptions), redisOptions);
        builder.Services.AddSingleton(redisOptions);

        #endregion

        #region Cahcing

        var redisCacheOptions = new RedisCacheOptions();
        builder.Configuration.Bind(nameof(redisCacheOptions), redisCacheOptions);
        builder.Services.AddSingleton(redisCacheOptions);

        if (redisCacheOptions.Enabled)
        {
            builder.Services.AddStackExchangeRedisCache(
                options =>
                    options.Configuration = builder.Configuration.GetConnectionString("RedisCache")
            );
            builder.Services.AddSingleton<IResponseCacheService, ResponseCacheService>();
        }

        #endregion

        var logger = new LoggerConfiguration().ReadFrom
            .Configuration(builder.Configuration)
            .Enrich.FromLogContext()
            .CreateLogger();

        builder.Logging.ClearProviders();
        builder.Logging.AddSerilog(logger);

        builder.Services.AddControllers(options =>
        {
            options.Conventions.Add(
                new RouteTokenTransformerConvention(new ToSlugCaseTransformerConvention())
            );
        });

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.AddSwagger();

        builder.Services.AddRouting(options => options.LowercaseUrls = true);

        builder.AddJwtAuthentication().AddPolicyBasedAuthorization();

        builder
            .AddNpgsql()
            .UseAffixForModelMapping("Model")
            .AddDapperSnakeCaseConvention()
            .AddRepositories()
            .AddMigrations();

        #region DbServices

        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
        builder.Services.AddScoped<IRoleService, RoleService>();
        builder.Services.AddScoped<IPermissionService, PermissionService>();
        builder.Services.AddScoped<IRolePermissionService, RolePermissionService>();
        builder.Services.AddScoped<IExtCredentialsService, ExtCredentialsService>();
        builder.Services.AddScoped<IAutoregistrarSettingsService, AutoregistrarSettingsService>();
        builder.Services.AddScoped<IIssueTypeService, IssueTypeService>();
        builder.Services.AddScoped<IIssueFieldService, IssueFieldService>();
        builder.Services.AddScoped<IRuleService, RuleService>();
        builder.Services.AddScoped<IIssueService, IssueService>();
        builder.Services.AddScoped<IQueryParametersService, QueryParametersService>();

        #endregion

        builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
        builder.Services.AddScoped<ICredentialsEncryptor, CredentialsEncryptor>();

        builder.Services.AddTransient<DatabaseSeeder>();

        builder.Services.AddSingleton<IMapper, Mapper>();
        builder.Services.AddScoped<IMappingHelper, MappingHelper>();

        builder.Services.AddGrpcClient<Autoregistrar.AutoregistrarClient>(options =>
        {
            options.Address = new Uri(
                builder.Configuration["AutoregistrarUri"]
                    ?? throw new NullConfigurationEntryException("Autoregistrar URI wasn't set")
            );
        });

        builder.Services.AddValidatorsFromAssemblyContaining<UserCredentialsValidator>();

        builder.Services.AddScoped<ITokenDb, TokenDb>();

        var app = builder.Build();

        app.ConfigureModelToDomainMapping();
        app.ConfigureDomainToModelMapping();
        app.ConfigureDomainToDtoMapping();

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

        using (var scope = app.Services.CreateScope())
        {
            var runner = scope.ServiceProvider.GetService<IMigrationRunner>();

            //runner!.MigrateUp();
            Console.WriteLine("MIGRATIONS SECTION");

            var seeder = scope.ServiceProvider.GetService<DatabaseSeeder>();

            if (seeder is null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("CRITICAL: No database seeder was found, exiting.");
                Console.ResetColor();
                throw new ArgumentNullException(nameof(seeder));
            }

            await seeder.SeedData();
        }

        await app.RunAsync();
    }
}