using Edvantix.Blog.Domain.AggregatesModel.TagAggregate;
using Edvantix.Chassis.Exceptions;
using MediatR;

namespace Edvantix.Blog.Features.TagFeature.Features.DeleteTag;

/// <summary>
/// Команда для удаления тега блога.
/// </summary>
public sealed record DeleteTagCommand(long TagId) : IRequest;

/// <summary>
/// Обработчик команды удаления тега.
/// </summary>
public sealed class DeleteTagCommandHandler(IServiceProvider provider)
    : IRequestHandler<DeleteTagCommand>
{
    public async Task Handle(DeleteTagCommand request, CancellationToken cancellationToken)
    {
        using var tagRepo = provider.GetRequiredService<ITagRepository>();

        var tag =
            await tagRepo.GetByIdAsync(request.TagId, cancellationToken)
            ?? throw new NotFoundException($"Тег с ID {request.TagId} не найден.");

        await tagRepo.DeleteAsync(tag, cancellationToken);
        await tagRepo.SaveEntitiesAsync(cancellationToken);
    }
}
