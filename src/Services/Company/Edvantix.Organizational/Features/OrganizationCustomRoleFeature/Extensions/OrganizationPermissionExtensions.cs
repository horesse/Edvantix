using Edvantix.Chassis.Utilities;
using Edvantix.Organizational.Domain.AggregatesModel.OrganizationCustomRoleAggregate;
using Edvantix.Organizational.Infrastructure.Services;
using Microsoft.AspNetCore.Http;

namespace Edvantix.Organizational.Features.OrganizationCustomRoleFeature.Extensions;

/// <summary>
/// Расширения для маршрутов Minimal API: декларативная проверка разрешений организации.
/// </summary>
/// <example>
/// <code>
/// app.MapGet("/organizations/{organizationId}/members", ...)
///    .RequireOrganizationPermission(Permission.MemberView)
///    .RequireAuthorization();
/// </code>
/// </example>
public static class OrganizationPermissionExtensions
{
    /// <summary>
    /// Добавляет фильтр проверки разрешения пользователя в контексте организации.
    /// Ожидает параметр маршрута <c>organizationId</c> (Guid).
    /// Возвращает 403, если у пользователя нет требуемого разрешения.
    /// </summary>
    /// <param name="builder">Построитель маршрута.</param>
    /// <param name="permission">Требуемое разрешение матрицы доступа.</param>
    public static RouteHandlerBuilder RequireOrganizationPermission(
        this RouteHandlerBuilder builder,
        Permission permission
    )
    {
        return builder.AddEndpointFilter(
            async (context, next) =>
            {
                var routeValue = context.HttpContext.GetRouteValue("organizationId");

                if (!Guid.TryParse(routeValue?.ToString(), out var organizationId))
                {
                    return Results.Problem(
                        "Параметр organizationId отсутствует или имеет неверный формат.",
                        statusCode: StatusCodes.Status400BadRequest
                    );
                }

                var profileId = context.HttpContext.RequestServices.TryGetProfileId();

                if (!profileId.HasValue)
                {
                    return Results.Json(
                        new { Detail = "Профиль пользователя не найден." },
                        statusCode: StatusCodes.Status403Forbidden
                    );
                }

                var permService =
                    context.HttpContext.RequestServices.GetRequiredService<IOrganizationPermissionService>();
                var hasPermission = await permService.HasPermissionAsync(
                    profileId.Value,
                    organizationId,
                    permission
                );

                if (!hasPermission)
                {
                    return Results.Json(
                        new
                        {
                            Detail = $"У вас нет разрешения '{permission}' в данной организации.",
                        },
                        statusCode: StatusCodes.Status403Forbidden
                    );
                }

                return await next(context);
            }
        );
    }
}
