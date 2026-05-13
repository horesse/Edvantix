using Edvantix.Organizational.Domain.AggregatesModel.GroupAggregate;

namespace Edvantix.Organizational.Features.Groups.SuggestCode;

public sealed class SuggestGroupCodeEndpoint : IEndpoint<Ok<string>, SuggestGroupCodeQuery, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/groups/suggest-code",
                async (
                    [AsParameters] SuggestGroupCodeQuery request,
                    ISender sender,
                    CancellationToken cancellationToken
                ) => await HandleAsync(request, sender, cancellationToken)
            )
            .WithName("SuggestGroupCode")
            .WithTags("Группы")
            .WithSummary("Предложить уникальный код для новой группы")
            .WithDescription(
                "Возвращает следующий свободный код вида B1-01, B1-02 … на основе существующих групп организации"
            )
            .Produces<string>()
            .Produces(StatusCodes.Status401Unauthorized)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<Ok<string>> HandleAsync(
        SuggestGroupCodeQuery request,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var code = await sender.Send(request, cancellationToken);
        return TypedResults.Ok(code);
    }
}
