using Npgsql;
using static EvAutoreg.Errors.ErrorCodes;

namespace EvAutoreg.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ErrorHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (NpgsqlException e)
        {
            context.Response.StatusCode = 500;
            var error = ErrorCode[9001];
            await context.Response.WriteAsJsonAsync(error);
        }
    }
}
