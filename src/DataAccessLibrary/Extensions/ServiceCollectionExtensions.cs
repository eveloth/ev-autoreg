using DataAccessLibrary.SqlDataAccess;
using Microsoft.Extensions.DependencyInjection;

namespace DataAccessLibrary.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureSqlDataAccess(this IServiceCollection services)
    {
        return services;
    }
    
    public static IServiceCollection UseAffixForModelMapping(this IServiceCollection services, string affix)
    {
        SqlDataAccessOptions.HasAffix = true;
        SqlDataAccessOptions.Affix = affix;

        return services;
    }
    
    public static IServiceCollection UseCustomSplitOn(this IServiceCollection services, string splitOn)
    {
        SqlDataAccessOptions.SplitOn = splitOn;
        
        return services;
    }
}