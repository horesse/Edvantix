using Edvantix.Blog.Domain.AggregatesModel.TagAggregate;
using Edvantix.Chassis.Exceptions;
using MediatR;

namespace Edvantix.Blog.Features.TagFeature.Features.UpdateTag;

/// <summary>
/// Команда для обновления тега блога.
/// </summary>
public sealed record UpdateTagCommand(long TagId, string Name, string Slug) : IRequest;

/// <summary>
/// Обработчик команды обновления тега.
/// </summary>
public sealed class UpdateTagCommandHandler(IServiceProvider provider)
    : IRequestHandler<UpdateTagCommand>
{
    public async Task Handle(UpdateTagCommand request, CancellationToken cancellationToken)
    {
        using var tagRepo = provider.GetRequiredService<ITagRepository>();

        var tag =
            await tagRepo.GetByIdAsync(request.TagId, cancellationToken)
            ?? throw new NotFoundException($"Тег с ID {request.TagId} не найден.");

        tag.Update(request.Name, request.Slug);

        await tagRepo.UpdateAsync(tag, cancellationToken);
        await tagRepo.SaveEntitiesAsync(cancellationToken);
    }
}
