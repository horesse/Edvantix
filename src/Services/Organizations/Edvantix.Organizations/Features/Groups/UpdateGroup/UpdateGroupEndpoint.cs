using Edvantix.Constants.Permissions;

namespace Edvantix.Organizations.Features.Groups.UpdateGroup;

/// <summary>PUT /v1/groups/{id} — updates an existing group's properties.</summary>
public sealed class UpdateGroupEndpoint : IEndpoint<NoContent, UpdateGroupCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(
                "/groups/{id:guid}",
                async (
                    Guid id,
                    UpdateGroupRequest body,
                    ISender sender,
                    CancellationToken cancellationToken
                ) =>
                    await HandleAsync(
                        new UpdateGroupCommand
                        {
                            Id = id,
                            Name = body.Name,
                            MaxCapacity = body.MaxCapacity,
                            Color = body.Color,
                        },
                        sender,
                        cancellationToken
                    )
            )
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesValidationProblem()
            .WithName("UpdateGroup")
            .WithTags("Groups")
            .WithSummary("Update a group")
            .WithDescription(
                "Updates the properties of an existing group. The group must belong to the current tenant."
            )
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization(GroupsPermissions.UpdateGroup);
    }

    public async Task<NoContent> HandleAsync(
        UpdateGroupCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(command, cancellationToken);

        return TypedResults.NoContent();
    }
}

/// <summary>Request body for the update-group endpoint.</summary>
public sealed record UpdateGroupRequest(string Name, int MaxCapacity, string Color);
