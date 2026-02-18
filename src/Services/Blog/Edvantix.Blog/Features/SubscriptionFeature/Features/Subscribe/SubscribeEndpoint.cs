using Edvantix.Blog.Domain.AggregatesModel.SubscriptionAggregate;
using Edvantix.Chassis.Endpoints;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Edvantix.Blog.Features.SubscriptionFeature.Features.Subscribe;

/// <summary>
/// Запрос на подписку от клиента.
/// </summary>
public sealed record SubscribeRequest(ContentSubscriptionType ContentTypes);

/// <summary>
/// Эндпоинт для создания или обновления подписки текущего пользователя на блог.
/// </summary>
public sealed class SubscribeEndpoint : IEndpoint<Ok<long>, SubscribeCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/subscriptions",
                async (SubscribeRequest request, ISender sender, CancellationToken ct) =>
                    await HandleAsync(new SubscribeCommand(request.ContentTypes), sender, ct)
            )
            .WithName("Subscribe")
            .WithTags("Subscriptions")
            .WithSummary("Подписаться на блог")
            .WithDescription(
                "Создаёт или обновляет подписку текущего пользователя на обновления блога. "
                    + "ContentTypes — битовая маска: 1 = News, 2 = Changelog, 3 = All."
            )
            .Produces<long>()
            .Produces(StatusCodes.Status400BadRequest)
            .RequireAuthorization();
    }

    public async Task<Ok<long>> HandleAsync(
        SubscribeCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var id = await sender.Send(command, cancellationToken);
        return TypedResults.Ok(id);
    }
}
