using Edvantix.Chassis.Endpoints;
using Edvantix.Constants.Core;
using Edvantix.ProfileService.Features.ProfileFeature.Models;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Edvantix.ProfileService.Features.ProfileFeature.UpdateProfileByAdmin;

/// <summary>
/// Эндпоинт для обновления профиля администратором
/// </summary>
public class UpdateProfileByAdminEndpoint
    : IEndpoint<NoContent, UpdateProfileByAdminCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(
                "/profile/{id:long}",
                async (long id, UpdateProfileModel model, ISender sender, CancellationToken ct) =>
                {
                    var command = new UpdateProfileByAdminCommand(id, model);
                    return await HandleAsync(command, sender, ct);
                }
            )
            .WithName("UpdateProfileByAdmin")
            .WithTags("Profile")
            .WithSummary("Обновить профиль (администратор)")
            .WithDescription("Позволяет администратору обновить профиль любого пользователя")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status403Forbidden)
            .RequireAuthorization(Authorization.Policies.Admin);
    }

    public async Task<NoContent> HandleAsync(
        UpdateProfileByAdminCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(command, cancellationToken);
        return TypedResults.NoContent();
    }
}
