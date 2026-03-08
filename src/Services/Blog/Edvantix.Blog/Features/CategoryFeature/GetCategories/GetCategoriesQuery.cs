namespace Edvantix.Blog.Features.CategoryFeature.GetCategories;

using Mediator;

/// <summary>
/// Запрос для получения полного списка категорий блога.
/// </summary>
public sealed record GetCategoriesQuery : IQuery<IReadOnlyList<CategoryModel>>;

/// <summary>
/// Обработчик запроса на получение всех категорий.
/// </summary>
internal sealed class GetCategoriesQueryHandler(IServiceProvider provider)
    : IQueryHandler<GetCategoriesQuery, IReadOnlyList<CategoryModel>>
{
    public async ValueTask<IReadOnlyList<CategoryModel>> Handle(
        GetCategoriesQuery request,
        CancellationToken cancellationToken
    )
    {
        var categoryRepo = provider.GetRequiredService<ICategoryRepository>();

        var categories = await categoryRepo.ListAsync(cancellationToken);

        var mapper = provider.GetRequiredService<IMapper<Category, CategoryModel>>();

        return [.. categories.Select(mapper.Map)];
    }
}
