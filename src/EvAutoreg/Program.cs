using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Text;
using Dapper;
using DataAccessLibrary.Extensions;
using DataAccessLibrary.Repository;
using DataAccessLibrary.Repository.Interfaces;
using DataAccessLibrary.SqlDataAccess;
using EvAutoreg.Exceptions;
using EvAutoreg.Mapping;
using EvAutoreg.Middleware;
using EvAutoreg.Services;
using EvAutoreg.Services.Interfaces;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Npgsql;
using NuGet.Protocol;
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

        #region Swagger

        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "EvAutoreg", Version = "v0.4.0" });

            options.AddSecurityDefinition(
                "Bearer",
                new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter an issued token",
                    Name = "Authrization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                }
            );

            options.AddSecurityRequirement(
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                }
            );

            var xmlCommentsFileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlCommentsFileName));
        });

        #endregion

        builder.Services.AddRouting(options => options.LowercaseUrls = true);

        #region JWT Authentication

        builder.Services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(t =>
            {
                t.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer =
                        builder.Configuration["Jwt:Issuer"]
                        ?? throw new NullConfigurationEntryException(
                            "Could not read configuration for JWT"
                        ),
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            builder.Configuration["Jwt:Key"]
                                ?? throw new NullConfigurationEntryException(
                                    "Could not read configuration for JWT"
                                )
                        )
                    ),
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = true
                };
            });

        #endregion

        #region Policy-based Authorization

        builder.Services.AddAuthorization(options =>
        {
            var userPermissions = Permissions.GetPermissions();

            foreach (var permission in userPermissions)
            {
                options.AddPolicy(
                    $"{permission.PermissionName}",
                    policy => policy.RequireClaim("Permission", $"{permission.PermissionName}")
                );
            }
        });

        #endregion

        #region SqlDataAccess

        builder.Services.AddScoped<IDbConnection>(
            _ => new NpgsqlConnection(builder.Configuration.GetConnectionString("Default"))
        );
        builder.Services.AddScoped<DbTransaction>(s =>
        {
            var connection = s.GetRequiredService<IDbConnection>();
            connection.Open();
            return (DbTransaction)connection.BeginTransaction();
        });

        builder.Services.ConfigureSqlDataAccess().UseAffixForModelMapping("Model");
        builder.Services.AddScoped<ISqlDataAccess, SqlDataAccess>();

        builder.Services.AddScoped<IGeneralPurposeRepository, GeneralPurposeRepository>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IRoleRepository, RoleRepository>();
        builder.Services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();
        builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
        builder.Services.AddScoped<IExtCredentialsRepository, ExtCredentialsRepository>();
        builder.Services.AddScoped<IIssueTypeRepository, IssueTypeRepository>();
        builder.Services.AddScoped<IIssueRepository, IssueRepository>();
        builder.Services.AddScoped<IRuleRepository, RuleRepository>();
        builder.Services.AddScoped<
            IEvApiQueryParametersRepository,
            EvApiQueryParametersRepository
        >();
        builder.Services.AddScoped<IMailAnalysisRulesRepository, MailAnalysisRulesRepository>();
        builder.Services.AddScoped<IIssueFieldRepository, IssueFieldRepository>();
        builder.Services.AddScoped<IUnitofWork, UnitofWork>();

        builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
        builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
        builder.Services.AddScoped<ICredentialsEncryptor, CredentialsEncryptor>();

        builder.Services.AddTransient<DatabaseSeeder>();

        #endregion

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

        DefaultTypeMap.MatchNamesWithUnderscores = true;

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