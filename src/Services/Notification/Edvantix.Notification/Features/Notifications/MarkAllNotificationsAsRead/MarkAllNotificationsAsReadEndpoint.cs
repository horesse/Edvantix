namespace Edvantix.Notification.Features.Notifications.MarkAllNotificationsAsRead;

public sealed class MarkAllNotificationsAsReadEndpoint
    : IEndpoint<NoContent, MarkAllNotificationsAsReadCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/notifications/read-all",
                async (ClaimsPrincipal user, ISender sender, CancellationToken cancellationToken) =>
                    await HandleAsync(
                        new MarkAllNotificationsAsReadCommand(
                            NotificationHelpers.GetAccountId(user)
                        ),
                        sender,
                        cancellationToken
                    )
            )
            .WithName("Пометить все уведомления прочитанными")
            .WithTags("Уведомления")
            .WithSummary("Пометить все как прочитанные")
            .WithDescription(
                "Помечает все непрочитанные уведомления текущего пользователя как прочитанные."
            )
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<NoContent> HandleAsync(
        MarkAllNotificationsAsReadCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(command, cancellationToken);
        return TypedResults.NoContent();
    }
}
