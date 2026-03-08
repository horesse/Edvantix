namespace Edvantix.Blog.Features.CategoryFeature.CreateCategory;

using Mediator;

/// <summary>
/// Команда для создания новой категории блога.
/// </summary>
public sealed record CreateCategoryCommand(string Name, string Slug, string? Description)
    : ICommand<Guid>;

/// <summary>
/// Обработчик команды создания категории.
/// </summary>
internal sealed class CreateCategoryCommandHandler(IServiceProvider provider)
    : ICommandHandler<CreateCategoryCommand, Guid>
{
    public async ValueTask<Guid> Handle(
        CreateCategoryCommand request,
        CancellationToken cancellationToken
    )
    {
        var category = new Category(request.Name, request.Slug, request.Description);

        var categoryRepo = provider.GetRequiredService<ICategoryRepository>();
        await categoryRepo.AddAsync(category, cancellationToken);
        await categoryRepo.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return category.Id;
    }
}
