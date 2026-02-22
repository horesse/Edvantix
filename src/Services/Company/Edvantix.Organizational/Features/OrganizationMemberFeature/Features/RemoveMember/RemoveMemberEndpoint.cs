namespace Edvantix.Organizational.Features.OrganizationMemberFeature.Features.RemoveMember;

public class RemoveMemberEndpoint : IEndpoint<NoContent, RemoveMemberCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete(
                "/organizations/{orgId:guid}/members/{memberId:guid}",
                async (Guid orgId, Guid memberId, ISender sender, CancellationToken ct) =>
                    await HandleAsync(new RemoveMemberCommand(orgId, memberId), sender, ct)
            )
            .WithName("RemoveOrganizationMember")
            .WithTags("Organization Members")
            .WithSummary("Удалить участника")
            .WithDescription(
                "Удаляет участника из организации (мягкое удаление). Доступно владельцу и менеджеру."
            )
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization();
    }

    public async Task<NoContent> HandleAsync(
        RemoveMemberCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(command, cancellationToken);
        return TypedResults.NoContent();
    }
}
