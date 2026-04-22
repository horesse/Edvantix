namespace Edvantix.Notification.Features.Notifications.GetUnreadCount;

public sealed class GetUnreadCountEndpoint
    : IEndpoint<Ok<UnreadCountViewModel>, GetUnreadCountQuery, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/notifications/unread-count",
                async (ClaimsPrincipal user, ISender sender, CancellationToken cancellationToken) =>
                    await HandleAsync(
                        new GetUnreadCountQuery(NotificationHelpers.GetProfileId(user)),
                        sender,
                        cancellationToken
                    )
            )
            .WithName("Количество непрочитанных уведомлений")
            .WithTags("Уведомления")
            .WithSummary("Количество непрочитанных")
            .WithDescription(
                "Возвращает количество непрочитанных уведомлений текущего пользователя."
            )
            .Produces<UnreadCountViewModel>()
            .Produces(StatusCodes.Status401Unauthorized)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

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
