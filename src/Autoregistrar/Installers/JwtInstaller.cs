using System.Text;
using Api.Options;
using Autoregistrar.Exceptions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Autoregistrar.Installers;

public static class JwtInstaller
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
}