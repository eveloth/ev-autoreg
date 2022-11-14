using System.Runtime.CompilerServices;
using DataAccessLibrary.SqlDataAccess;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace DataAccessLibrary.Extensions;

public static class ApplicationExtensions
{
    public static IServiceCollection ConfigureSqlDataAccess(this IServiceCollection services)
    {
        return services;
    }
    
    public static IServiceCollection UseAffixForModelMapping2(this IServiceCollection services, string affix)
    {
        SqlDataAccessOptions.HasAffix = true;
        SqlDataAccessOptions.Affix = affix;

        return services;
    }
    
    public static IServiceCollection UseCustomSplitOn2(this IServiceCollection services, string splitOn)
    {
        SqlDataAccessOptions.SplitOn = splitOn;
        
        return services;
    }
}