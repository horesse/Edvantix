using Edvantix.Chassis.Endpoints;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Edvantix.Blog.Features.SubscriptionFeature.Features.Unsubscribe;

/// <summary>
/// Эндпоинт для отмены подписки текущего пользователя на блог.
/// </summary>
public sealed class UnsubscribeEndpoint : IEndpoint<NoContent, UnsubscribeCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete(
                "/subscriptions",
                async (ISender sender, CancellationToken ct) =>
                    await HandleAsync(new UnsubscribeCommand(), sender, ct)
            )
            .WithName("Unsubscribe")
            .WithTags("Subscriptions")
            .WithSummary("Отписаться от блога")
            .WithDescription("Удаляет подписку текущего пользователя на обновления блога.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization();
    }

    public async Task<NoContent> HandleAsync(
        UnsubscribeCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(command, cancellationToken);
        return TypedResults.NoContent();
    }
}
