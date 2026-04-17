namespace Edvantix.Organizational.Features.OrganizationMembers.Update;

public sealed class UpdateOrganizationMemberEndpoint
    : IEndpoint<NoContent, UpdateOrganizationMemberCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(
                "/organizations/{organizationId:guid}/members/{id:guid}",
                async (
                    UpdateOrganizationMemberCommand command,
                    ISender sender,
                    CancellationToken cancellationToken
                ) => await HandleAsync(command, sender, cancellationToken)
            )
            .WithName("UpdateOrganizationMember")
            .WithTags("Участники организации")
            .WithSummary("Изменить роль участника организации")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status422UnprocessableEntity)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<NoContent> HandleAsync(
        UpdateOrganizationMemberCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(command, cancellationToken);
        return TypedResults.NoContent();
    }
}
