namespace Edvantix.Organizations.Features.Roles.UpdateRole;

/// <summary>PUT /v1/roles/{id} — renames an existing role.</summary>
public sealed class UpdateRoleEndpoint : IEndpoint<NoContent, UpdateRoleCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(
                "/roles/{id:guid}",
                async (
                    Guid id,
                    UpdateRoleRequest body,
                    ISender sender,
                    CancellationToken cancellationToken
                ) =>
                    await HandleAsync(
                        new UpdateRoleCommand { Id = id, Name = body.Name },
                        sender,
                        cancellationToken
                    )
            )
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status422UnprocessableEntity)
            .WithName("UpdateRole")
            .WithTags("Roles")
            .WithSummary("Update role name")
            .WithDescription(
                "Renames an existing role. The role must belong to the current tenant."
            )
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<NoContent> HandleAsync(
        UpdateRoleCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(command, cancellationToken);
        return TypedResults.NoContent();
    }
}

/// <summary>Request body for the update-role endpoint.</summary>
public sealed record UpdateRoleRequest(string Name);
