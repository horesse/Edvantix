namespace Edvantix.Organizational.Features.OrganizationMembers.Delete;

public sealed class DeleteOrganizationMemberEndpoint : IEndpoint<NoContent, Guid, Guid, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete(
                "/organizations/{organizationId:guid}/members/{id:guid}",
                async (
                    Guid organizationId,
                    Guid id,
                    ISender sender,
                    CancellationToken cancellationToken
                ) => await HandleAsync(organizationId, id, sender, cancellationToken)
            )
            .WithName("DeleteOrganizationMember")
            .WithTags("Участники организации")
            .WithSummary("Удалить участника из организации")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<NoContent> HandleAsync(
        Guid organizationId,
        Guid id,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(
            new DeleteOrganizationMemberCommand(organizationId, id),
            cancellationToken
        );
        return TypedResults.NoContent();
    }
}
