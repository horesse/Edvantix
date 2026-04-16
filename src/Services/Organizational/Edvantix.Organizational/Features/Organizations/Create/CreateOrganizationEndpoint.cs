namespace Edvantix.Organizational.Features.Organizations.Create;

public sealed class CreateOrganizationEndpoint
    : IEndpoint<Created<Guid>, CreateOrganizationCommand, ISender, LinkGenerator>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/organizations",
                async (
                    CreateOrganizationCommand command,
                    ISender sender,
                    LinkGenerator linker,
                    CancellationToken cancellationToken
                ) => await HandleAsync(command, sender, linker, cancellationToken)
            )
            .WithName("CreateOrganization")
            .WithTags("Организации")
            .WithSummary("Создание организации")
            .WithDescription(
                "Создаёт организацию, базовые роли и назначает текущего пользователя владельцем."
            )
            .Produces<Guid>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status422UnprocessableEntity)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<Created<Guid>> HandleAsync(
        CreateOrganizationCommand command,
        ISender sender,
        LinkGenerator linker,
        CancellationToken cancellationToken = default
    )
    {
        var id = await sender.Send(command, cancellationToken);
        var location =
            linker.GetPathByName("GetOrganizationById", new { id }) ?? $"/api/organizations/{id}";

        return TypedResults.Created(location, id);
    }
}
