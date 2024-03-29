using EvAutoreg.Api.Extensions;
using EvAutoreg.Api.Contracts.Responses;
using EvAutoreg.Api.Exceptions;
using FluentValidation;
using Grpc.Core;
using Npgsql;
using StackExchange.Redis;
using static EvAutoreg.Api.Errors.ErrorCodes;

namespace EvAutoreg.Api.Middleware;

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
        catch (ValidationException e)
        {
            var error = e.ToErrorResponse();
            context.Response.StatusCode = 400;
            await context.Response.WriteAsJsonAsync(error);
        }
        catch (ApiException e)
        {
            var error = e.ToErrorResponse();

            context.Response.StatusCode = error is null
                ? 500
                : error.ApiError.ErrorCode.ToString().EndsWith('4')
                    ? 404
                    : 400;

            await context.Response.WriteAsJsonAsync(error);
        }
        catch (NpgsqlException e)
        {
            _logger.LogError("An error occured during sql transaction: {ErrorMessage}", e);
            context.Response.StatusCode = 500;
            var error = new ErrorResponse { ApiError = ErrorCode[13001] };
            await context.Response.WriteAsJsonAsync(error);
        }
        catch (NullConfigurationEntryException e)
        {
            _logger.LogCritical(
                "An error occured while reading configuration file, service might not be operational! "
                    + "Error Message: {ErrorMessage}",
                e
            );
            context.Response.StatusCode = 500;
            var error = new ErrorResponse { ApiError = ErrorCode[14001] };
            await context.Response.WriteAsJsonAsync(error);
        }
        catch (RpcException e)
        {
            _logger.LogError(
                "Autoregistrar service returned an error: {StatusCode}: {Error}",
                e.StatusCode,
                e.Message
            );

            context.Response.StatusCode = 500;

            var error = e.StatusCode switch
            {
                StatusCode.FailedPrecondition => new ErrorResponse { ApiError = ErrorCode[13002] },
                _ => new ErrorResponse { ApiError = ErrorCode[13001] }
            };
            await context.Response.WriteAsJsonAsync(error);
        }
        catch (RedisTimeoutException e)
        {
            _logger.LogError("An error occured while trying to communicate with redis: {Error}", e);
            context.Response.StatusCode = 500;
            var error = new ErrorResponse { ApiError = ErrorCode[13001] };
            await context.Response.WriteAsJsonAsync(error);
        }
        catch (Exception e)
        {
            _logger.LogError("An error occured executing the request: {Error}", e);
            context.Response.StatusCode = 500;
            var error = new ErrorResponse { ApiError = ErrorCode[13001] };
            await context.Response.WriteAsJsonAsync(error);
        }
    }
}