using Edvantix.Chassis.Endpoints;
using Edvantix.Constants.Other;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Edvantix.Company.Features.ContactFeature.Features.UpdateContact;

public sealed record UpdateContactRequest(ContactType Type, string Value, string? Description);

public class UpdateContactEndpoint : IEndpoint<NoContent, UpdateContactCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(
                "/organizations/{orgId:long}/contacts/{contactId:long}",
                async (long orgId, long contactId, UpdateContactRequest request, ISender sender, CancellationToken ct) =>
                {
                    var command = new UpdateContactCommand(orgId, contactId, request.Type, request.Value, request.Description);
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
