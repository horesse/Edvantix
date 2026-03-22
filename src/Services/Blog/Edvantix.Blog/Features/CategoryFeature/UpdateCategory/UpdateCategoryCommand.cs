using Edvantix.Chassis.Utilities.Guards;

namespace Edvantix.Blog.Features.CategoryFeature.UpdateCategory;

public sealed record UpdateCategoryCommand(
    Guid CategoryId,
    string Name,
    string Slug,
    string? Description
) : ICommand;

public sealed class UpdateCategoryCommandHandler(ICategoryRepository categoryRepository)
    : ICommandHandler<UpdateCategoryCommand>
{
    public async ValueTask<Unit> Handle(
        UpdateCategoryCommand request,
        CancellationToken cancellationToken
    )
    {
        var category = await categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);

        Guard.Against.NotFound(category, request.CategoryId);

        category.Update(request.Name, request.Slug, request.Description);

        await categoryRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
