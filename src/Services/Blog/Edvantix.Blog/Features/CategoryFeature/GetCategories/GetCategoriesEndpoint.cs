namespace Edvantix.Blog.Features.CategoryFeature.GetCategories;

/// <summary>
/// Публичный эндпоинт для получения списка категорий блога.
/// </summary>
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
            .WithName("GetCategories")
            .WithTags("Categories")
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
