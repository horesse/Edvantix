namespace Edvantix.Notification.Features.Notifications.MarkNotificationAsRead;

public sealed class MarkNotificationAsReadEndpoint
    : IEndpoint<Results<NoContent, NotFound>, MarkNotificationAsReadCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/notifications/{id:guid}/read",
                async (
                    Guid id,
                    ClaimsPrincipal user,
                    ISender sender,
                    CancellationToken cancellationToken
                ) =>
                    await HandleAsync(
                        new MarkNotificationAsReadCommand(
                            id,
                            NotificationHelpers.GetProfileId(user)
                        ),
                        sender,
                        cancellationToken
                    )
            )
            .WithName("Пометить уведомление прочитанным")
            .WithTags("Уведомления")
            .WithSummary("Пометить как прочитанное")
            .WithDescription(
                "Помечает уведомление как прочитанное. Проверяет принадлежность уведомления текущему пользователю."
            )
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<Results<NoContent, NotFound>> HandleAsync(
        MarkNotificationAsReadCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var found = await sender.Send(command, cancellationToken);
        return found ? TypedResults.NoContent() : TypedResults.NotFound();
    }
}
