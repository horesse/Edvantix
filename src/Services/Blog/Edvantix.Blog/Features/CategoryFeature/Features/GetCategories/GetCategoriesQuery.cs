using Edvantix.Blog.Domain.AggregatesModel.CategoryAggregate;
using Edvantix.Blog.Features.CategoryFeature.Models;
using MediatR;

namespace Edvantix.Blog.Features.CategoryFeature.Features.GetCategories;

/// <summary>
/// Запрос для получения полного списка категорий блога.
/// </summary>
public sealed record GetCategoriesQuery : IRequest<IReadOnlyList<CategoryModel>>;

/// <summary>
/// Обработчик запроса на получение всех категорий.
/// </summary>
public sealed class GetCategoriesQueryHandler(IServiceProvider provider)
    : IRequestHandler<GetCategoriesQuery, IReadOnlyList<CategoryModel>>
{
    public async Task<IReadOnlyList<CategoryModel>> Handle(
        GetCategoriesQuery request,
        CancellationToken cancellationToken
    )
    {
        using var categoryRepo = provider.GetRequiredService<ICategoryRepository>();

        var categories = await categoryRepo.GetAllAsync(cancellationToken);

        return categories
            .Select(c => new CategoryModel
            {
                Id = c.Id,
                Name = c.Name,
                Slug = c.Slug,
                Description = c.Description,
                CreatedAt = c.CreatedAt,
            })
            .ToList();
    }
}
