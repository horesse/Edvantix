using Edvantix.Blog.Domain.AggregatesModel.CategoryAggregate;
using Edvantix.Chassis.Exceptions;
using MediatR;

namespace Edvantix.Blog.Features.CategoryFeature.Features.UpdateCategory;

/// <summary>
/// Команда для обновления категории блога.
/// </summary>
public sealed record UpdateCategoryCommand(
    long CategoryId,
    string Name,
    string Slug,
    string? Description
) : IRequest;

/// <summary>
/// Обработчик команды обновления категории.
/// </summary>
public sealed class UpdateCategoryCommandHandler(IServiceProvider provider)
    : IRequestHandler<UpdateCategoryCommand>
{
    public async Task Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        using var categoryRepo = provider.GetRequiredService<ICategoryRepository>();

        var category =
            await categoryRepo.GetByIdAsync(request.CategoryId, cancellationToken)
            ?? throw new NotFoundException($"Категория с ID {request.CategoryId} не найдена.");

        category.Update(request.Name, request.Slug, request.Description);

        await categoryRepo.UpdateAsync(category, cancellationToken);
        await categoryRepo.SaveEntitiesAsync(cancellationToken);
    }
}
