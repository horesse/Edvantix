namespace Edvantix.Organizational.Features.OrganizationFeature.Features.CreateOrganization;

public class CreateOrganizationEndpoint
    : IEndpoint<Created<Guid>, CreateOrganizationCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/organizations",
                async (CreateOrganizationCommand command, ISender sender, CancellationToken ct) =>
                    await HandleAsync(command, sender, ct)
            )
            .WithName("CreateOrganization")
            .WithTags("Organizations")
            .WithSummary("Создать организацию")
            .WithDescription(
                "Создаёт новую организацию. Текущий пользователь становится владельцем."
            )
            .Produces<Guid>(StatusCodes.Status201Created)
            .RequireAuthorization();
    }

    public async Task<Created<Guid>> HandleAsync(
        CreateOrganizationCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var id = await sender.Send(command, cancellationToken);
        return TypedResults.Created($"/organizations/{id}", id);
    }
}
