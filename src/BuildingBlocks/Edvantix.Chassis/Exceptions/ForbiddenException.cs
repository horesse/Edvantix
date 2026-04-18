using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.Buffering;
using Microsoft.Extensions.Logging;

namespace Edvantix.Chassis.Exceptions;

/// <summary>Выбрасывается, когда профиль не имеет необходимого разрешения.</summary>
public sealed class ForbiddenException(string message) : Exception(message);

internal sealed class ForbiddenExceptionHandler(
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
        if (exception is not ForbiddenException forbidden)
        {
            return false;
        }

        logger.LogWarning(
            exception,
            "[{Handler}] Forbidden: {Message}",
            nameof(ForbiddenExceptionHandler),
            forbidden.Message
        );

        logBuffer.Flush();

        await TypedResults
            .Problem(statusCode: 403, title: "Access denied.")
            .ExecuteAsync(httpContext);

        return true;
    }
}

public static class ForbiddenExceptionHandlerExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Регистрирует <see cref="ForbiddenExceptionHandler"/> в конвейере обработки исключений ASP.NET Core.
        /// </summary>
        public void AddForbiddenExceptionHandler()
        {
            services.AddExceptionHandler<ForbiddenExceptionHandler>();
        }
    }
}
