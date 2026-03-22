namespace Edvantix.Blog.Features.CategoryFeature.CreateCategory;

public sealed record CreateCategoryCommand(string Name, string Slug, string? Description)
    : ICommand<Guid>;

internal sealed class CreateCategoryCommandHandler(ICategoryRepository categoryRepository)
    : ICommandHandler<CreateCategoryCommand, Guid>
{
    public async ValueTask<Guid> Handle(
        CreateCategoryCommand request,
        CancellationToken cancellationToken
    )
    {
        var category = new Category(request.Name, request.Slug, request.Description);

        await categoryRepository.AddAsync(category, cancellationToken);
        await categoryRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return category.Id;
    }
}
