using Edvantix.Blog.Features.SubscriptionFeature.Models;
using Edvantix.Chassis.Endpoints;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Edvantix.Blog.Features.SubscriptionFeature.Features.GetMySubscription;

/// <summary>
/// Эндпоинт для получения настроек подписки текущего пользователя.
/// </summary>
public sealed class GetMySubscriptionEndpoint
    : IEndpoint<Ok<SubscriptionModel>, GetMySubscriptionQuery, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/subscriptions/me",
                async (ISender sender, CancellationToken ct) =>
                    await HandleAsync(new GetMySubscriptionQuery(), sender, ct)
            )
            .WithName("GetMySubscription")
            .WithTags("Subscriptions")
            .WithSummary("Моя подписка")
            .WithDescription("Возвращает настройки подписки текущего пользователя на блог.")
            .Produces<SubscriptionModel>()
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization();
    }

    public async Task<Ok<SubscriptionModel>> HandleAsync(
        GetMySubscriptionQuery query,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(query, cancellationToken);
        return TypedResults.Ok(result);
    }
}
