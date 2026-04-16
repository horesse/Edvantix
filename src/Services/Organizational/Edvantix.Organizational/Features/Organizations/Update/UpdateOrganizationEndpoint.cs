namespace Edvantix.Organizational.Features.Organizations.Update;

public sealed class UpdateOrganizationEndpoint
    : IEndpoint<NoContent, UpdateOrganizationCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch(
                "/organizations/{id:guid}",
                async (
                    Guid id,
                    UpdateOrganizationCommand command,
                    ISender sender,
                    CancellationToken cancellationToken
                ) => await HandleAsync(command with { Id = id }, sender, cancellationToken)
            )
            .WithName("UpdateOrganization")
            .WithTags("Организации")
            .WithSummary("Редактировать организацию")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status422UnprocessableEntity)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<NoContent> HandleAsync(
        UpdateOrganizationCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(command, cancellationToken);
        return TypedResults.NoContent();
    }
}
