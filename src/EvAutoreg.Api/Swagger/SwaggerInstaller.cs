﻿using System.Reflection;
using EvAutoreg.Api.Swagger.Examples.Requests;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

namespace EvAutoreg.Api.Swagger;

public static class SwaggerInstaller
{
    public static WebApplicationBuilder AddSwagger(this WebApplicationBuilder builder)
    {
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "EvAutoreg", Version = "v1.0.0" });

            options.AddSecurityDefinition(
                "Bearer",
                new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter an issued token",
                    Name = "Authorization",
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

            options.ExampleFilters();
            options.SupportNonNullableReferenceTypes();
            options.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();
        });

        builder.Services.AddSwaggerExamplesFromAssemblyOf<CredentialsRequestExample>();

        return builder;
    }
}
