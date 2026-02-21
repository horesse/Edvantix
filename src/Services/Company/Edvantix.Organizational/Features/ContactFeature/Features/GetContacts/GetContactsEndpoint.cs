using Edvantix.Organizational.Features.ContactFeature.Models;

namespace Edvantix.Organizational.Features.ContactFeature.Features.GetContacts;

public class GetContactsEndpoint
    : IEndpoint<Ok<IEnumerable<ContactModel>>, GetContactsQuery, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/organizations/{orgId:long}/contacts",
                async (long orgId, ISender sender, CancellationToken ct) =>
                    await HandleAsync(new GetContactsQuery(orgId), sender, ct)
            )
            .WithName("GetOrganizationContacts")
            .WithTags("Contacts")
            .WithSummary("Контакты организации")
            .WithDescription("Возвращает список контактов организации. Доступно участникам.")
            .Produces<IEnumerable<ContactModel>>()
            .Produces(StatusCodes.Status403Forbidden)
            .RequireAuthorization();
    }

    public async Task<Ok<IEnumerable<ContactModel>>> HandleAsync(
        GetContactsQuery query,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(query, cancellationToken);
        return TypedResults.Ok(result);
    }
}
