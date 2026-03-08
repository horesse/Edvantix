namespace Edvantix.Notification.Features.Notifications.MarkNotificationAsRead;

/// <summary>
/// POST /notifications/{id}/read — пометить одно уведомление как прочитанное.
/// </summary>
public sealed class MarkNotificationAsReadEndpoint
    : IEndpoint<Results<NoContent, NotFound>, MarkNotificationAsReadCommand, ISender>
{
    /// <inheritdoc />
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/notifications/{id:guid}/read",
                async (Guid id, ClaimsPrincipal user, ISender sender, CancellationToken ct) =>
                    await HandleAsync(
                        new MarkNotificationAsReadCommand(
                            id,
                            NotificationHelpers.GetAccountId(user)
                        ),
                        sender,
                        ct
                    )
            )
            .WithName("MarkNotificationAsRead")
            .WithTags("Notifications")
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

    /// <inheritdoc />
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
