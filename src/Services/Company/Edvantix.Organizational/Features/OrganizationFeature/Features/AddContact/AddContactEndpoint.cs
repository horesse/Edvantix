using Edvantix.Constants.Other;

namespace Edvantix.Organizational.Features.OrganizationFeature.Features.AddContact;

public sealed record AddContactRequest(ContactType Type, string Value, string? Description);

public class AddContactEndpoint : IEndpoint<Created<ulong>, AddContactCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/organizations/{orgId:long}/contacts",
                async (
                    long orgId,
                    AddContactRequest request,
                    ISender sender,
                    CancellationToken ct
                ) =>
                {
                    var command = new AddContactCommand(
                        (ulong)orgId,
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
            .Produces<ulong>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization();
    }

    public async Task<Created<ulong>> HandleAsync(
        AddContactCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var id = await sender.Send(command, cancellationToken);
        return TypedResults.Created($"/organizations/{command.OrganizationId}/contacts/{id}", id);
    }
}
