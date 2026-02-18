using Edvantix.Blog.Domain.AggregatesModel.CategoryAggregate;
using Edvantix.Chassis.Exceptions;
using MediatR;

namespace Edvantix.Blog.Features.CategoryFeature.Features.DeleteCategory;

/// <summary>
/// Команда для физического удаления категории блога.
/// </summary>
public sealed record DeleteCategoryCommand(long CategoryId) : IRequest;

/// <summary>
/// Обработчик команды удаления категории.
/// </summary>
public sealed class DeleteCategoryCommandHandler(IServiceProvider provider)
    : IRequestHandler<DeleteCategoryCommand>
{
    public async Task Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        using var categoryRepo = provider.GetRequiredService<ICategoryRepository>();

        var category =
            await categoryRepo.GetByIdAsync(request.CategoryId, cancellationToken)
            ?? throw new NotFoundException($"Категория с ID {request.CategoryId} не найдена.");

        await categoryRepo.DeleteAsync(category, cancellationToken);
        await categoryRepo.SaveEntitiesAsync(cancellationToken);
    }
}
