using Edvantix.Constants.Permissions;

namespace Edvantix.Organizations.Features.Groups.CreateGroup;

/// <summary>POST /v1/groups — creates a new group scoped to the current tenant.</summary>
public sealed class CreateGroupEndpoint
    : IEndpoint<Created<Guid>, CreateGroupCommand, ISender, LinkGenerator>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/groups",
                async (
                    CreateGroupCommand command,
                    ISender sender,
                    LinkGenerator linker,
                    CancellationToken cancellationToken
                ) => await HandleAsync(command, sender, linker, cancellationToken)
            )
            .Produces<Guid>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status409Conflict)
            .ProducesValidationProblem()
            .WithName("CreateGroup")
            .WithTags("Groups")
            .WithSummary("Create a group")
            .WithDescription(
                "Creates a new named group scoped to the tenant from the X-School-Id header."
            )
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization(GroupsPermissions.CreateGroup);
    }

    public async Task<Created<Guid>> HandleAsync(
        CreateGroupCommand command,
        ISender sender,
        LinkGenerator linker,
        CancellationToken cancellationToken = default
    )
    {
        var id = await sender.Send(command, cancellationToken);
        var location = linker.GetPathByName("GetGroups", null) ?? $"/api/v1/groups";

        return TypedResults.Created(location, id);
    }
}
