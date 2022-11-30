using DataAccessLibrary.Extensions;
using EvAutoreg.Extensions;
using EvAutoreg.Mapping;
using EvAutoreg.Middleware;
using EvAutoreg.Services;
using EvAutoreg.Services.Interfaces;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Serilog;

namespace EvAutoreg;

internal static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

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
            .AddRepositories();

        builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
        builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
        builder.Services.AddScoped<ICredentialsEncryptor, CredentialsEncryptor>();

        builder.Services.AddTransient<DatabaseSeeder>();

        builder.Services.AddSingleton<IMapper, Mapper>();
        builder.Services.AddGrpcClient<Registrar.RegistrarClient>(options =>
        {
            options.Address = new Uri("https://localhost:7037");
        });

        var app = builder.Build();

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