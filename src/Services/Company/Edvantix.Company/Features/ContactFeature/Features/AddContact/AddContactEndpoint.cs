using Edvantix.Chassis.Endpoints;
using Edvantix.Constants.Other;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Edvantix.Company.Features.ContactFeature.Features.AddContact;

public sealed record AddContactRequest(ContactType Type, string Value, string? Description);

public class AddContactEndpoint : IEndpoint<Created<long>, AddContactCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/organizations/{orgId:long}/contacts",
                async (long orgId, AddContactRequest request, ISender sender, CancellationToken ct) =>
                {
                    var command = new AddContactCommand(orgId, request.Type, request.Value, request.Description);
                    return await HandleAsync(command, sender, ct);
                }
            )
            .WithName("AddOrganizationContact")
            .WithTags("Contacts")
            .WithSummary("Добавить контакт")
            .WithDescription("Добавляет контакт организации. Доступно владельцу и менеджеру.")
            .Produces<long>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status403Forbidden)
            .RequireAuthorization();
    }

    public async Task<Created<long>> HandleAsync(
        AddContactCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var id = await sender.Send(command, cancellationToken);
        return TypedResults.Created($"/organizations/{command.OrganizationId}/contacts/{id}", id);
    }
}
