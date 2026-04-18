namespace Edvantix.Organizational.Features.OrganizationMembers.Delete;

public sealed class DeleteOrganizationMemberEndpoint : IEndpoint<NoContent, Guid, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete(
                "/members/{id:guid}",
                async (
                    Guid id,
                    ISender sender,
                    CancellationToken cancellationToken
                ) => await HandleAsync(id, sender, cancellationToken)
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
        Guid id,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(new DeleteOrganizationMemberCommand(id), cancellationToken);
        return TypedResults.NoContent();
    }
}
