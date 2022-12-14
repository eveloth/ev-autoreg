using System.Security.Claims;

namespace Api.Extensions;

public static class HttpContextExtensions
{
    public static int GetUserId(this HttpContext context)
    {
        var id = context.User.Claims.Single(x => x.Type == ClaimTypes.NameIdentifier).Value;
        return int.Parse(id);
    }
}