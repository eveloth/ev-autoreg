using System.Data;
using System.Data.Common;
using Dapper;
using DataAccessLibrary.Repository;
using DataAccessLibrary.Repository.Interfaces;
using DataAccessLibrary.SqlDataAccess;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace DataAccessLibrary.Extensions;

public static class WebApplicationBuilderDataExtensions
{
    public static WebApplicationBuilder AddNpgsql(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IDbConnection>(
            _ => new NpgsqlConnection(builder.Configuration.GetConnectionString("Default"))
        );
        builder.Services.AddScoped<DbTransaction>(s =>
        {
            var connection = s.GetRequiredService<IDbConnection>();
            connection.Open();
            return (DbTransaction)connection.BeginTransaction();
        });

        return builder;
    }

    public static WebApplicationBuilder AddDapperSnakeCaseConvention(
        this WebApplicationBuilder builder
    )
    {
        DefaultTypeMap.MatchNamesWithUnderscores = true;
        return builder;
    }

    public static WebApplicationBuilder UseAffixForModelMapping(
        this WebApplicationBuilder builder,
        string affix
    )
    {
        SqlDataAccessOptions.HasAffix = true;
        SqlDataAccessOptions.Affix = affix;

        return builder;
    }

    public static WebApplicationBuilder UseCustomSplitOn(
        this WebApplicationBuilder builder,
        string splitOn
    )
    {
        SqlDataAccessOptions.SplitOn = splitOn;

        return builder;
    }

    public static WebApplicationBuilder AddRepositories(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<ISqlDataAccess, SqlDataAccess.SqlDataAccess>();

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

        return builder;
    }
}