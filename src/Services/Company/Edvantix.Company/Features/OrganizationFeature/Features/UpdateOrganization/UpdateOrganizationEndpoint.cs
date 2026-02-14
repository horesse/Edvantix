using Edvantix.Chassis.Endpoints;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Edvantix.Company.Features.OrganizationFeature.Features.UpdateOrganization;

public sealed record UpdateOrganizationRequest(
    string Name,
    string NameLatin,
    string ShortName,
    string? PrintName,
    string? Description
);

public class UpdateOrganizationEndpoint
    : IEndpoint<NoContent, UpdateOrganizationCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(
                "/organizations/{id:long}",
                async (long id, UpdateOrganizationRequest request, ISender sender, CancellationToken ct) =>
                {
                    var command = new UpdateOrganizationCommand(
                        id,
                        request.Name,
                        request.NameLatin,
                        request.ShortName,
                        request.PrintName,
                        request.Description
                    );
                    return await HandleAsync(command, sender, ct);
                }
            )
            .WithName("UpdateOrganization")
            .WithTags("Organizations")
            .WithSummary("Обновить организацию")
            .WithDescription("Обновляет информацию об организации. Доступно владельцу и менеджеру.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization();
    }

    public async Task<NoContent> HandleAsync(
        UpdateOrganizationCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(command, cancellationToken);
        return TypedResults.NoContent();
    }
}
