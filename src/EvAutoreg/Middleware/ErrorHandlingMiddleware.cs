using EvAutoreg.Exceptions;
using Npgsql;
using static EvAutoreg.Errors.ErrorCodes;

namespace EvAutoreg.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (NpgsqlException e)
        {
            _logger.LogError("An error occured during sql transaction: {ErrorMessage}", e);
            context.Response.StatusCode = 500;
            var error = ErrorCode[9001];
            await context.Response.WriteAsJsonAsync(error);
        }
        catch (NullConfigurationEntryException e)
        {
            _logger.LogCritical(
                "An error occured while reading configuration file, service might not be operational! " +
                "Error Message: {ErrorMessage}",
                e
            );
            context.Response.StatusCode = 500;
            var error = ErrorCode[10001];
            await context.Response.WriteAsJsonAsync(error);
        }
    }
}
