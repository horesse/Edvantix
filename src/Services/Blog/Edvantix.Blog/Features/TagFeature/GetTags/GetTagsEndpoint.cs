namespace Edvantix.Blog.Features.TagFeature.GetTags;

/// <summary>
/// Публичный эндпоинт для получения списка тегов блога.
/// </summary>
public sealed class GetTagsEndpoint : IEndpoint<Ok<IReadOnlyList<TagModel>>, GetTagsQuery, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/tags",
                async (ISender sender, CancellationToken cancellationToken) =>
                    await HandleAsync(new GetTagsQuery(), sender, cancellationToken)
            )
            .WithName("GetTags")
            .WithTags("Tags")
            .WithSummary("Список тегов")
            .WithDescription("Возвращает все теги блога.")
            .Produces<IReadOnlyList<TagModel>>()
            .AllowAnonymous();
    }

    public async Task<Ok<IReadOnlyList<TagModel>>> HandleAsync(
        GetTagsQuery query,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(query, cancellationToken);
        return TypedResults.Ok(result);
    }
}
