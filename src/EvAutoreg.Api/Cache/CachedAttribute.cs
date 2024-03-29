using System.Text;
using EvAutoreg.Api.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace EvAutoreg.Api.Cache;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class CachedAttribute : Attribute, IAsyncActionFilter
{
    private readonly int _ttlSec;

    public CachedAttribute(int ttlSec)
    {
        _ttlSec = ttlSec;
    }

    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next
    )
    {
        //before
        var cacheSettings = context.HttpContext.RequestServices
            .GetRequiredService<IOptions<RedisCacheOptions>>()
            .Value;

        if (!cacheSettings.Enabled)
        {
            await next();
            return;
        }

        var cacheService =
            context.HttpContext.RequestServices.GetRequiredService<IResponseCacheService>();
        var cacheKey = GenerateCacheKey(context.HttpContext.Request);
        var cachedResponse = await cacheService.GetCachedResponse(cacheKey);

        if (!string.IsNullOrEmpty(cachedResponse))
        {
            var result = new ContentResult
            {
                Content = cachedResponse,
                ContentType = "application/json",
                StatusCode = 200,
            };

            context.Result = result;
            return;
        }

        var executedContext = await next();

        if (executedContext.Result is OkObjectResult okObjectResult)
        {
            await cacheService.CacheResponse(
                cacheKey,
                okObjectResult.Value!,
                TimeSpan.FromSeconds(_ttlSec)
            );
        }
    }

    private static string GenerateCacheKey(HttpRequest request)
    {
        var keyBuilder = new StringBuilder();
        keyBuilder.Append($"{request.Path}");

        foreach (var (key, value) in request.Query.OrderBy(x => x.Key))
        {
            keyBuilder.Append($"|{key}-{value}");
        }

        return keyBuilder.ToString();
    }
}