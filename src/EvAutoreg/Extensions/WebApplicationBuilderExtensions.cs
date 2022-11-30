using System.Reflection;
using System.Text;
using EvAutoreg.Exceptions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace EvAutoreg.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddSwagger(this WebApplicationBuilder builder)
    {

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

        return builder;
    }

    public static WebApplicationBuilder AddJwtAuthentication(this WebApplicationBuilder builder)
    {

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

        return builder;
    }

    public static WebApplicationBuilder AddPolicyBasedAuthorization(this WebApplicationBuilder builder)
    {
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

        return builder;
    }
}