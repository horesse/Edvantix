namespace Edvantix.Blog.Features.CategoryFeature.DeleteCategory;

/// <summary>
/// Команда для физического удаления категории блога.
/// </summary>
public sealed record DeleteCategoryCommand(Guid CategoryId) : IRequest;

/// <summary>
/// Обработчик команды удаления категории.
/// </summary>
internal sealed class DeleteCategoryCommandHandler(IServiceProvider provider)
    : IRequestHandler<DeleteCategoryCommand>
{
    public async ValueTask<Unit> Handle(
        DeleteCategoryCommand request,
        CancellationToken cancellationToken
    )
    {
        var categoryRepo = provider.GetRequiredService<ICategoryRepository>();

        var category =
            await categoryRepo.GetByIdAsync(request.CategoryId, cancellationToken)
            ?? throw new NotFoundException($"Категория с ID {request.CategoryId} не найдена.");

        await categoryRepo.DeleteAsync(category, cancellationToken);
        await categoryRepo.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
