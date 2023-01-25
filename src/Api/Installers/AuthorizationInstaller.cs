﻿using Api.Seeding;

namespace Api.Installers;

public static class AuthorizationInstaller
{
    public static WebApplicationBuilder AddPolicyBasedAuthorization(
        this WebApplicationBuilder builder
    )
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