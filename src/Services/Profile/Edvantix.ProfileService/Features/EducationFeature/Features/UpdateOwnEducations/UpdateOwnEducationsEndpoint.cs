using Edvantix.Chassis.Endpoints;
using Edvantix.ProfileService.Features.EducationFeature.Models;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Edvantix.ProfileService.Features.EducationFeature.Features.UpdateOwnEducations;

/// <summary>
/// Эндпоинт для обновления образования текущего пользователя
/// </summary>
public class UpdateOwnEducationsEndpoint : IEndpoint<NoContent, UpdateOwnEducationsCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(
                "/profile/educations",
                async (
                    IEnumerable<EducationModel> educations,
                    ISender sender,
                    CancellationToken ct
                ) =>
                {
                    var command = new UpdateOwnEducationsCommand(educations);
                    return await HandleAsync(command, sender, ct);
                }
            )
            .WithName("UpdateOwnEducations")
            .WithTags("Profile")
            .WithSummary("Обновить образование текущего пользователя")
            .WithDescription("Позволяет пользователю обновить своё образование")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized)
            .RequireAuthorization();
    }

    public async Task<NoContent> HandleAsync(
        UpdateOwnEducationsCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(command, cancellationToken);
        return TypedResults.NoContent();
    }
}
