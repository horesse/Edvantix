using Edvantix.Constants.Other;

namespace Edvantix.Organizational.Features.OrganizationFeature.Features.UpdateContact;

public sealed record UpdateContactRequest(ContactType Type, string Value, string? Description);

public sealed class UpdateContactEndpoint : IEndpoint<NoContent, UpdateContactCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(
                "/organizations/{orgId:guid}/contacts/{contactId:guid}",
                async (
                    Guid orgId,
                    Guid contactId,
                    UpdateContactRequest request,
                    ISender sender,
                    CancellationToken ct
                ) =>
                {
                    var command = new UpdateContactCommand(
                        orgId,
                        contactId,
                        request.Type,
                        request.Value,
                        request.Description
                    );
                    return await HandleAsync(command, sender, ct);
                }
            )
            .WithName("UpdateOrganizationContact")
            .WithTags("Contacts")
            .WithSummary("Обновить контакт")
            .WithDescription("Обновляет контакт организации. Доступно владельцу и менеджеру.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization();
    }

    public async Task<NoContent> HandleAsync(
        UpdateContactCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(command, cancellationToken);
        return TypedResults.NoContent();
    }
}
