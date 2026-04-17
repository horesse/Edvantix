namespace Edvantix.Organizational.Features.OrganizationMembers.Create;

public sealed class CreateOrganizationMemberEndpoint
    : IEndpoint<Created<Guid>, CreateOrganizationMemberCommand, ISender, LinkGenerator>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/organizations/{organizationId:guid}/members",
                async (
                    CreateOrganizationMemberCommand command,
                    ISender sender,
                    LinkGenerator linker,
                    CancellationToken cancellationToken
                ) => await HandleAsync(command, sender, linker, cancellationToken)
            )
            .WithName("CreateOrganizationMember")
            .WithTags("Участники организации")
            .WithSummary("Добавить участника в организацию")
            .Produces<Guid>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status422UnprocessableEntity)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<Created<Guid>> HandleAsync(
        CreateOrganizationMemberCommand command,
        ISender sender,
        LinkGenerator linker,
        CancellationToken cancellationToken = default
    )
    {
        var id = await sender.Send(command, cancellationToken);
        var location =
            linker.GetPathByName(
                "GetOrganizationMemberById",
                new { organizationId = command.OrganizationId, id }
            ) ?? $"/api/organizations/{command.OrganizationId}/members/{id}";

        return TypedResults.Created(location, id);
    }
}
