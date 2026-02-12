using Edvantix.Chassis.Endpoints;
using Edvantix.ProfileService.Features.EmploymentHistoryFeature.Models;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Edvantix.ProfileService.Features.EmploymentHistoryFeature.Features.UpdateOwnEmploymentHistories;

/// <summary>
/// Эндпоинт для обновления истории трудоустройства текущего пользователя
/// </summary>
public class UpdateOwnEmploymentHistoriesEndpoint
    : IEndpoint<NoContent, UpdateOwnEmploymentHistoriesCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(
                "/profile/employment-histories",
                async (
                    [FromBody] IEnumerable<EmploymentHistoryModel> employmentHistories,
                    ISender sender,
                    CancellationToken ct
                ) =>
                {
                    var command = new UpdateOwnEmploymentHistoriesCommand(employmentHistories);
                    return await HandleAsync(command, sender, ct);
                }
            )
            .WithName("UpdateOwnEmploymentHistories")
            .WithTags("Profile")
            .WithSummary("Обновить историю трудоустройства текущего пользователя")
            .WithDescription("Позволяет пользователю обновить свою историю трудоустройства")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized)
            .RequireAuthorization();
    }

    public async Task<NoContent> HandleAsync(
        UpdateOwnEmploymentHistoriesCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(command, cancellationToken);
        return TypedResults.NoContent();
    }
}
