namespace Edvantix.Notification.Features.Notifications.GetUnreadCount;

/// <summary>
/// GET /notifications/unread-count — количество непрочитанных уведомлений (для бейджа в шапке).
/// </summary>
public sealed class GetUnreadCountEndpoint
    : IEndpoint<Ok<UnreadCountViewModel>, GetUnreadCountQuery, ISender>
{
    /// <inheritdoc />
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/notifications/unread-count",
                async (ClaimsPrincipal user, ISender sender, CancellationToken ct) =>
                    await HandleAsync(
                        new GetUnreadCountQuery(NotificationHelpers.GetAccountId(user)),
                        sender,
                        ct
                    )
            )
            .WithName("GetUnreadNotificationsCount")
            .WithTags("Notifications")
            .WithSummary("Количество непрочитанных")
            .WithDescription(
                "Возвращает количество непрочитанных уведомлений текущего пользователя."
            )
            .Produces<UnreadCountViewModel>()
            .Produces(StatusCodes.Status401Unauthorized)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    /// <inheritdoc />
    public async Task<Ok<UnreadCountViewModel>> HandleAsync(
        GetUnreadCountQuery query,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(query, cancellationToken);
        return TypedResults.Ok(result);
    }
}
