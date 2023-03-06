using EvAutoreg.Api.Seeding;
using EvAutoreg.Api.Services;
using EvAutoreg.Api.Services.Interfaces;

namespace EvAutoreg.Api.Installers;

public static class ServicesInstaller
{
    public static WebApplicationBuilder InstallServices(this WebApplicationBuilder builder)
    {
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
        builder.Services.AddScoped<IIssueService, IssueService>();
        builder.Services.AddScoped<IQueryParametersService, QueryParametersService>();
        builder.Services.AddScoped<IRuleSetService, RuleSetService>();

        #endregion

        builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
        builder.Services.AddScoped<ICredentialsEncryptor, CredentialsEncryptor>();

        builder.Services.AddScoped<IAutoregistrarCallerService, AutoregistrarCallerService>();

        builder.Services.AddTransient<DatabaseSeeder>();

        return builder;
    }
}