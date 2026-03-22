using Edvantix.Chassis.Utilities.Guards;

namespace Edvantix.Blog.Features.CategoryFeature.DeleteCategory;

public sealed record DeleteCategoryCommand(Guid CategoryId) : ICommand;

internal sealed class DeleteCategoryCommandHandler(ICategoryRepository categoryRepository)
    : ICommandHandler<DeleteCategoryCommand>
{
    public async ValueTask<Unit> Handle(
        DeleteCategoryCommand request,
        CancellationToken cancellationToken
    )
    {
        var category = await categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);

        Guard.Against.NotFound(category, request.CategoryId);

        await categoryRepository.DeleteAsync(category, cancellationToken);
        await categoryRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
