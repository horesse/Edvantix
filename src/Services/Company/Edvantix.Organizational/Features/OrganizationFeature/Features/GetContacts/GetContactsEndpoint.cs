using Edvantix.Organizational.Features.OrganizationFeature.Models;

namespace Edvantix.Organizational.Features.OrganizationFeature.Features.GetContacts;

public sealed class GetContactsEndpoint
    : IEndpoint<Ok<IEnumerable<ContactModel>>, GetContactsQuery, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/organizations/{orgId:guid}/contacts",
                async (Guid orgId, ISender sender, CancellationToken ct) =>
                    await HandleAsync(new GetContactsQuery(orgId), sender, ct)
            )
            .WithName("GetOrganizationContacts")
            .WithTags("Contacts")
            .WithSummary("Контакты организации")
            .WithDescription("Возвращает список контактов организации. Доступно участникам.")
            .Produces<IEnumerable<ContactModel>>()
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
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
