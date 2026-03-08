namespace Edvantix.Blog.Features.CategoryFeature.UpdateCategory;
using Mediator;

/// <summary>
/// Команда для обновления категории блога.
/// </summary>
public sealed record UpdateCategoryCommand(
    Guid CategoryId,
    string Name,
    string Slug,
    string? Description
) : ICommand;

/// <summary>
/// Обработчик команды обновления категории.
/// </summary>
public sealed class UpdateCategoryCommandHandler(IServiceProvider provider)
    : ICommandHandler<UpdateCategoryCommand>
{
    public async ValueTask<Unit> Handle(
        UpdateCategoryCommand request,
        CancellationToken cancellationToken
    )
    {
        var categoryRepo = provider.GetRequiredService<ICategoryRepository>();

        var category =
            await categoryRepo.GetByIdAsync(request.CategoryId, cancellationToken)
            ?? throw new NotFoundException($"Категория с ID {request.CategoryId} не найдена.");

        category.Update(request.Name, request.Slug, request.Description);

        await categoryRepo.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
