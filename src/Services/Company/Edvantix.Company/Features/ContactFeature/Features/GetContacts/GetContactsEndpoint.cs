using Edvantix.Chassis.Endpoints;
using Edvantix.Company.Features.ContactFeature.Models;
using Edvantix.SharedKernel.Results;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Edvantix.Company.Features.ContactFeature.Features.GetContacts;

public class GetContactsEndpoint
    : IEndpoint<Ok<PagedResult<ContactModel>>, GetContactsQuery, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/organizations/contacts",
                async (
                    [AsParameters] GetContactsQuery query,
                    ISender sender,
                    CancellationToken ct
                ) => await HandleAsync(query, sender, ct)
            )
            .WithName("GetOrganizationContacts")
            .WithTags("Contacts")
            .WithSummary("Контакты организации")
            .WithDescription(
                "Возвращает пагинированный список контактов организации. Доступно участникам."
            )
            .WithPaginationHeaders()
            .Produces<PagedResult<ContactModel>>()
            .Produces(StatusCodes.Status403Forbidden)
            .RequireAuthorization();
    }

    public async Task<Ok<PagedResult<ContactModel>>> HandleAsync(
        GetContactsQuery query,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(query, cancellationToken);
        return TypedResults.Ok(result);
    }
}
