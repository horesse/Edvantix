namespace Edvantix.Blog.Features.CategoryFeature.GetCategories;

public sealed record GetCategoriesQuery : IQuery<IReadOnlyList<CategoryModel>>;

internal sealed class GetCategoriesQueryHandler(
    ICategoryRepository categoryRepository,
    IMapper<Category, CategoryModel> mapper
) : IQueryHandler<GetCategoriesQuery, IReadOnlyList<CategoryModel>>
{
    public async ValueTask<IReadOnlyList<CategoryModel>> Handle(
        GetCategoriesQuery request,
        CancellationToken cancellationToken
    )
    {
        var categories = await categoryRepository.ListAsync(cancellationToken);

        return [.. categories.Select(mapper.Map)];
    }
}
