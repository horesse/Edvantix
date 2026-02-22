namespace Edvantix.Blog.Features.CategoryFeature.CreateCategory;

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
    public async ValueTask<ulong> Handle(
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
