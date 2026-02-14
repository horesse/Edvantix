using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.Buffering;
using Microsoft.Extensions.Logging;

namespace Edvantix.Chassis.Exceptions;

public sealed class ForbiddenException(string message) : Exception(message)
{
    public static ForbiddenException For(string action) =>
        new($"Доступ запрещён для выполнения действия: {action}.");
}

public sealed class ForbiddenExceptionHandler(
    ILogger<ForbiddenExceptionHandler> logger,
    PerRequestLogBuffer logBuffer
) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken
    )
    {
        if (exception is not ForbiddenException forbiddenException)
        {
            return false;
        }

        logger.LogWarning(
            exception,
            "[{Handler}] Forbidden exception occurred: {Message}",
            nameof(ForbiddenExceptionHandler),
            forbiddenException.Message
        );

        logBuffer.Flush();

        httpContext.Response.StatusCode = StatusCodes.Status403Forbidden;

        await TypedResults
            .Json(
                new { Detail = forbiddenException.Message },
                statusCode: StatusCodes.Status403Forbidden
            )
            .ExecuteAsync(httpContext);

        return true;
    }
}
