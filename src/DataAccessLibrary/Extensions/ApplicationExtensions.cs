using DataAccessLibrary.SqlDataAccess;
using Microsoft.AspNetCore.Builder;

namespace DataAccessLibrary.Extensions;

public static class ApplicationExtensions
{
    public static void UseAffixForDbMapping(this IApplicationBuilder app, ISqlDataAccess db, string affix)
    {
        db.HasAffix = true;
        db.Affix = affix;
    }
    
}