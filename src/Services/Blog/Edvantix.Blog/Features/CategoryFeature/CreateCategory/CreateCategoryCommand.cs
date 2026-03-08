namespace Edvantix.Blog.Features.CategoryFeature.CreateCategory;

/// <summary>
/// Команда для создания новой категории блога.
/// </summary>
public sealed record CreateCategoryCommand(string Name, string Slug, string? Description)
    : IRequest<Guid>;

/// <summary>
/// Обработчик команды создания категории.
/// </summary>
internal sealed class CreateCategoryCommandHandler(IServiceProvider provider)
    : IRequestHandler<CreateCategoryCommand, Guid>
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
