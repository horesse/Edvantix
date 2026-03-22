namespace Edvantix.Blog.Features.CategoryFeature.GetCategories;

public sealed class GetCategoriesEndpoint
    : IEndpoint<Ok<IReadOnlyList<CategoryModel>>, GetCategoriesQuery, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/categories",
                async (ISender sender, CancellationToken cancellationToken) =>
                    await HandleAsync(new GetCategoriesQuery(), sender, cancellationToken)
            )
            .WithName("Список категорий")
            .WithTags("Категории")
            .WithSummary("Список категорий")
            .WithDescription("Возвращает все категории блога.")
            .Produces<IReadOnlyList<CategoryModel>>()
            .AllowAnonymous();
    }

    public async Task<Ok<IReadOnlyList<CategoryModel>>> HandleAsync(
        GetCategoriesQuery query,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(query, cancellationToken);
        return TypedResults.Ok(result);
    }
}
