namespace Edvantix.Organizational.Features.Organizations.Archive;

public sealed class ArchiveOrganizationEndpoint : IEndpoint<NoContent, Guid, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/organizations/{id:guid}/archive",
                async (Guid id, ISender sender, CancellationToken cancellationToken) =>
                    await HandleAsync(id, sender, cancellationToken)
            )
            .WithName("ArchiveOrganization")
            .WithTags("Организации")
            .WithSummary("Архивировать организацию")
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
        await sender.Send(new ArchiveOrganizationCommand(id), cancellationToken);
        return TypedResults.NoContent();
    }
}
