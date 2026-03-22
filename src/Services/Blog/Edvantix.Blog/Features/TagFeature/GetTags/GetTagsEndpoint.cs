namespace Edvantix.Blog.Features.TagFeature.GetTags;

public sealed class GetTagsEndpoint : IEndpoint<Ok<IReadOnlyList<TagModel>>, GetTagsQuery, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/tags",
                async (ISender sender, CancellationToken cancellationToken) =>
                    await HandleAsync(new GetTagsQuery(), sender, cancellationToken)
            )
            .WithName("Список тегов")
            .WithTags("Теги")
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
