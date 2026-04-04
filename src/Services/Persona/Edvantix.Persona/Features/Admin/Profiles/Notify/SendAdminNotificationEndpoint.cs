namespace Edvantix.Persona.Features.Admin.Profiles.Notify;

public sealed class SendAdminNotificationEndpoint
    : IEndpoint<NoContent, (Guid ProfileId, SendAdminNotificationRequest Request), ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/admin/profiles/{profileId:guid}/notify",
                async (
                    Guid profileId,
                    SendAdminNotificationRequest request,
                    ISender sender,
                    CancellationToken cancellationToken
                ) => await HandleAsync((profileId, request), sender, cancellationToken)
            )
            .WithName("Отправить уведомление пользователю (админ)")
            .WithTags("Администрирование")
            .WithSummary("Отправка in-app уведомления пользователю")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization(Authorization.Policies.Admin);
    }

    public async Task<NoContent> HandleAsync(
        (Guid ProfileId, SendAdminNotificationRequest Request) input,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var command = new SendAdminNotificationCommand(
            input.ProfileId,
            input.Request.Title,
            input.Request.Message,
            input.Request.Type
        );
        await sender.Send(command, cancellationToken);
        return TypedResults.NoContent();
    }
}
