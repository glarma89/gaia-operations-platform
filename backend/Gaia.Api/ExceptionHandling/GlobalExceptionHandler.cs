
using Gaia.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Gaia.Api.ExceptionHandling;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        int statusCode = exception switch
        {
            OperationValidationException =>
                StatusCodes.Status400BadRequest,

            OperationNotFoundException =>
                StatusCodes.Status404NotFound,

            _ => StatusCodes.Status500InternalServerError
        };

        if (statusCode == StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(
                exception,
                "Unhandled exception while processing {Path}",
                httpContext.Request.Path);
        }

        string title = statusCode switch
        {
            StatusCodes.Status400BadRequest => "Invalid operation input",
            StatusCodes.Status404NotFound => "Operation not found",
            _ => "Internal server error"
        };

        string detail = statusCode ==
            StatusCodes.Status500InternalServerError
                ? "An unexpected error occurred."
                : exception.Message;

        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = httpContext.Request.Path
        };

        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsJsonAsync(
            problem,
            cancellationToken);

        return true;
    }
}
