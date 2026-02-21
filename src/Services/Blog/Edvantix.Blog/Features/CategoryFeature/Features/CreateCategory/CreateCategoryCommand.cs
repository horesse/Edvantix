using Edvantix.Blog.Domain.AggregatesModel.CategoryAggregate;
using MediatR;

namespace Edvantix.Blog.Features.CategoryFeature.Features.CreateCategory;

/// <summary>
/// Команда для создания новой категории блога.
/// </summary>
public sealed record CreateCategoryCommand(string Name, string Slug, string? Description)
    : IRequest<ulong>;

/// <summary>
/// Обработчик команды создания категории.
/// </summary>
public sealed class CreateCategoryCommandHandler(IServiceProvider provider)
    : IRequestHandler<CreateCategoryCommand, ulong>
{
    public async Task<ulong> Handle(
        CreateCategoryCommand request,
        CancellationToken cancellationToken
    )
    {
        var category = new Category(request.Name, request.Slug, request.Description);

        using var categoryRepo = provider.GetRequiredService<ICategoryRepository>();
        await categoryRepo.InsertAsync(category, cancellationToken);
        await categoryRepo.SaveEntitiesAsync(cancellationToken);

        return category.Id;
    }
}
