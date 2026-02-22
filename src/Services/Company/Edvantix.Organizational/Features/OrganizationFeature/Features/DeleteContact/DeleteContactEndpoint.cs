namespace Edvantix.Organizational.Features.OrganizationFeature.Features.DeleteContact;

public class DeleteContactEndpoint : IEndpoint<NoContent, DeleteContactCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete(
                "/organizations/{orgId:guid}/contacts/{contactId:guid}",
                async (Guid orgId, Guid contactId, ISender sender, CancellationToken ct) =>
                    await HandleAsync(new DeleteContactCommand(orgId, contactId), sender, ct)
            )
            .WithName("DeleteOrganizationContact")
            .WithTags("Contacts")
            .WithSummary("Удалить контакт")
            .WithDescription("Удаляет контакт организации. Доступно владельцу и менеджеру.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization();
    }

    public async Task<NoContent> HandleAsync(
        DeleteContactCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(command, cancellationToken);
        return TypedResults.NoContent();
    }
}
