using System.Runtime.CompilerServices;
using DataAccessLibrary.SqlDataAccess;
using Microsoft.AspNetCore.Builder;

namespace DataAccessLibrary.Extensions;

public static class ApplicationExtensions
{
    private static ISqlDataAccess _db;
    public static IApplicationBuilder UseDataAccess(this IApplicationBuilder app, ISqlDataAccess db)
    {
        _db = db;

        return app;
    }
    public static IApplicationBuilder UseAffixForDbMapping(this IApplicationBuilder app, string affix)
    {
        _db.HasAffix = true;
        _db.Affix = affix;

        return app;
    }

    public static IApplicationBuilder UseCustomSplitOn(this IApplicationBuilder app, string splitOn)
    {
        _db.SplitOn = splitOn;
        return app;
    }
    
}