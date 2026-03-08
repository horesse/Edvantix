using Edvantix.Constants.Other;

namespace Edvantix.Organizational.Features.OrganizationFeature.Features.AddContact;

public sealed record AddContactRequest(ContactType Type, string Value, string? Description);

public sealed class AddContactEndpoint : IEndpoint<Created<Guid>, AddContactCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/organizations/{orgId:guid}/contacts",
                async (
                    Guid orgId,
                    AddContactRequest request,
                    ISender sender,
                    CancellationToken ct
                ) =>
                {
                    var command = new AddContactCommand(
                        orgId,
                        request.Type,
                        request.Value,
                        request.Description
                    );
                    return await HandleAsync(command, sender, ct);
                }
            )
            .WithName("AddOrganizationContact")
            .WithTags("Contacts")
            .WithSummary("Добавить контакт")
            .WithDescription("Добавляет контакт организации. Доступно владельцу и менеджеру.")
            .Produces<Guid>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization();
    }

    public async Task<Created<Guid>> HandleAsync(
        AddContactCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var id = await sender.Send(command, cancellationToken);
        return TypedResults.Created($"/organizations/{command.OrganizationId}/contacts/{id}", id);
    }
}
