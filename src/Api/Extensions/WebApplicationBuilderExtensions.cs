using System.Reflection;
using System.Text;
using Api.Exceptions;
using Api.Options;
using Api.Swagger.Examples.Requests;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

namespace Api.Extensions;

public static class WebApplicationBuilderExtensions
{


    public static WebApplicationBuilder AddJwtAuthentication(this WebApplicationBuilder builder)
    {
        var jwt = new JwtOptions();
        builder.Configuration.Bind(nameof(jwt), jwt);
        builder.Services.AddSingleton(jwt);

        var tokenValidationParameters = new TokenValidationParameters
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
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidateAudience = false,
            RequireExpirationTime = false
        };

        builder.Services.AddSingleton(tokenValidationParameters);

        builder.Services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(t =>
            {
                t.TokenValidationParameters = tokenValidationParameters;
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