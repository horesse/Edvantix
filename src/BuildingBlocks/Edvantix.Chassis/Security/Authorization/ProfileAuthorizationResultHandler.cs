using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;

namespace Edvantix.Chassis.Security.Authorization;

/// <summary>
/// Перехватывает отказы авторизации и возвращает <c>403 Forbidden</c>
/// с кодом <c>PROFILE_NOT_REGISTERED</c>, если причина — отсутствие профиля в Persona.
/// Все остальные случаи делегируются стандартному обработчику.
/// </summary>
internal sealed class ProfileAuthorizationResultHandler : IAuthorizationMiddlewareResultHandler
{
    private static readonly AuthorizationMiddlewareResultHandler Default = new();

    private static readonly byte[] ProfileNotRegisteredBody = JsonSerializer.SerializeToUtf8Bytes(
        new
        {
            code = "PROFILE_NOT_REGISTERED",
            detail = "Зарегистрируйте профиль в Persona-сервисе перед доступом к этому ресурсу.",
        }
    );

    public async Task HandleAsync(
        RequestDelegate next,
        HttpContext context,
        AuthorizationPolicy policy,
        PolicyAuthorizationResult authorizeResult
    )
    {
        if (
            authorizeResult.Forbidden
            && authorizeResult
                .AuthorizationFailure?.FailureReasons.OfType<ProfileNotRegisteredFailureReason>()
                .Any() == true
        )
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";
            await context.Response.Body.WriteAsync(ProfileNotRegisteredBody);
            return;
        }

        await Default.HandleAsync(next, context, policy, authorizeResult);
    }
}
