using System.Security.Claims;
using Edvantix.Chassis.Endpoints;
using Edvantix.Notification.Infrastructure;
using Edvantix.SharedKernel.Results;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Edvantix.Notification.Features.Notifications;

/// <summary>
/// REST-эндпоинты in-app уведомлений для клиентского фронтенда.
/// Аутентификация по JWT (Keycloak). Идентификатор пользователя извлекается из claim'а "sub".
/// </summary>
public sealed class GetNotificationsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/notifications",
                async (
                    ClaimsPrincipal user,
                    InAppNotificationService service,
                    [FromQuery] int pageIndex = 1,
                    [FromQuery] int pageSize = 20,
                    [FromQuery] bool? isRead = null,
                    CancellationToken ct = default
                ) =>
                {
                    var accountId = GetAccountId(user);
                    var clampedPageSize = Math.Clamp(pageSize, 1, 100);
                    var clampedPageIndex = Math.Max(pageIndex, 1);

                    var result = await service.GetAsync(
                        accountId,
                        clampedPageIndex,
                        clampedPageSize,
                        isRead,
                        ct
                    );

                    var viewModels = result
                        .Select(NotificationViewModel.FromDomain)
                        .ToList();

                    return TypedResults.Ok(
                        new PagedResult<NotificationViewModel>(
                            viewModels,
                            result.PageIndex,
                            result.PageSize,
                            result.TotalItems
                        )
                    );
                }
            )
            .WithName("GetNotifications")
            .WithTags("Notifications")
            .WithSummary("Получить уведомления пользователя")
            .WithDescription("Возвращает страницу in-app уведомлений текущего пользователя")
            .WithPaginationHeaders()
            .Produces<PagedResult<NotificationViewModel>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .MapToApiVersion(new(1, 0))
            .RequireAuthorization();
    }
}

/// <summary>
/// Возвращает количество непрочитанных уведомлений (для бейджа в шапке).
/// </summary>
public sealed class GetUnreadCountEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/notifications/unread-count",
                async (
                    ClaimsPrincipal user,
                    InAppNotificationService service,
                    CancellationToken ct = default
                ) =>
                {
                    var accountId = GetAccountId(user);
                    var count = await service.GetUnreadCountAsync(accountId, ct);

                    return TypedResults.Ok(new { Count = count });
                }
            )
            .WithName("GetUnreadNotificationsCount")
            .WithTags("Notifications")
            .WithSummary("Получить количество непрочитанных уведомлений")
            .Produces<object>()
            .Produces(StatusCodes.Status401Unauthorized)
            .MapToApiVersion(new(1, 0))
            .RequireAuthorization();
    }
}

/// <summary>
/// Помечает одно уведомление как прочитанное.
/// </summary>
public sealed class MarkNotificationAsReadEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/notifications/{id:guid}/read",
                async (
                    Guid id,
                    ClaimsPrincipal user,
                    InAppNotificationService service,
                    CancellationToken ct = default
                ) =>
                {
                    var accountId = GetAccountId(user);
                    var found = await service.MarkAsReadAsync(id, accountId, ct);

                    return found
                        ? Results.NoContent()
                        : Results.NotFound();
                }
            )
            .WithName("MarkNotificationAsRead")
            .WithTags("Notifications")
            .WithSummary("Пометить уведомление как прочитанное")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized)
            .MapToApiVersion(new(1, 0))
            .RequireAuthorization();
    }
}

/// <summary>
/// Помечает все уведомления пользователя как прочитанные.
/// </summary>
public sealed class MarkAllNotificationsAsReadEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/notifications/read-all",
                async (
                    ClaimsPrincipal user,
                    InAppNotificationService service,
                    CancellationToken ct = default
                ) =>
                {
                    var accountId = GetAccountId(user);
                    await service.MarkAllAsReadAsync(accountId, ct);

                    return Results.NoContent();
                }
            )
            .WithName("MarkAllNotificationsAsRead")
            .WithTags("Notifications")
            .WithSummary("Пометить все уведомления как прочитанные")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .MapToApiVersion(new(1, 0))
            .RequireAuthorization();
    }
}

file static class EndpointHelpers
{
    /// <summary>
    /// Извлекает Keycloak account_id из claim'а "sub" JWT-токена.
    /// </summary>
    internal static Guid GetAccountId(ClaimsPrincipal user)
    {
        var sub =
            user.FindFirstValue("sub")
            ?? throw new UnauthorizedAccessException("Пользователь не аутентифицирован.");

        return Guid.TryParse(sub, out var id)
            ? id
            : throw new UnauthorizedAccessException(
                "Некорректный формат идентификатора пользователя."
            );
    }
}
