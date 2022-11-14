using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Text;
using Dapper;
using DataAccessLibrary.Extensions;
using DataAccessLibrary.Repositories;
using DataAccessLibrary.SqlDataAccess;
using EvAutoreg.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Npgsql;
using Serilog;

namespace EvAutoreg;
internal static class Program
{
    public static async Task Main(string [] args)
    {
        var builder = WebApplication.CreateBuilder(args);
    
        // Add services to the container.
    
        var logger = new LoggerConfiguration().ReadFrom
            .Configuration(builder.Configuration)
            .Enrich.FromLogContext()
            .CreateLogger();
    
        builder.Logging.ClearProviders();
        builder.Logging.AddSerilog(logger);
    
        builder.Services.AddControllers();
    
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
    
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
    
        builder.Services.AddRouting(options => options.LowercaseUrls = true);
    
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
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])
                    ),
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = true
                };
            });
    
        // builder.Services.AddAuthorization(options =>
        // {
        //     options.AddPolicy("CanDoStuff", policy => policy.RequireClaim("Permission", "CanDoStuff") );
        //
        //     var userPermissions = Permissions.GetPermissions<Permissions.UserPermission>();
        //
        //     foreach (var permissionName in userPermissions)
        //     {
        //         options.AddPolicy($"{permissionName}Permission", policy => policy.RequireClaim("Permission", $"{permissionName}Permission"));
        //     }
        // });

        builder.Services.AddScoped<IDbConnection>(_ =>
            new NpgsqlConnection(builder.Configuration.GetConnectionString("Default")));
        builder.Services.AddScoped<DbTransaction>(s =>
        {
            var connection = s.GetRequiredService<IDbConnection>();
            connection.Open();
            return (DbTransaction)connection.BeginTransaction();
        });

        builder.Services.ConfigureSqlDataAccess().UseAffixForModelMapping2("Model");
        builder.Services.AddScoped<ISqlDataAccess, SqlDataAccess>();

        builder.Services.AddScoped<IGeneralPurposeRepository, GeneralPurposeRepository>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IUserRolesRepository, UserRolesRepository>();
        builder.Services.AddScoped<IAccessControlRepository, AccessControlRepository>();
        builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
        builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

        builder.Services.AddTransient<DatabaseSeeder>();
    
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
    
        app.UseHttpsRedirection();
    
        app.UseAuthentication();
        app.UseAuthorization();
    
        app.MapControllers();

        using (var scope = app.Services.CreateScope())
        {
            var seeder = scope.ServiceProvider.GetService<DatabaseSeeder>();

            seeder.SeedData();
        }
    
        DefaultTypeMap.MatchNamesWithUnderscores = true;
    
        await app.RunAsync();
    }
}