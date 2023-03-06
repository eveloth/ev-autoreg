using System.Text;
using EvAutoreg.Api.Exceptions;
using EvAutoreg.Api.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace EvAutoreg.Api.Installers;

public static class JwtInstaller
{
    public static WebApplicationBuilder AddJwtAuthentication(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddOptions<JwtOptions>()
            .Bind(builder.Configuration.GetSection(JwtOptions.Jwt))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        var jwtOptions = builder.Configuration
            .GetRequiredSection(JwtOptions.Jwt)
            .Get<JwtOptions>()!;

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = jwtOptions.Issuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),
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