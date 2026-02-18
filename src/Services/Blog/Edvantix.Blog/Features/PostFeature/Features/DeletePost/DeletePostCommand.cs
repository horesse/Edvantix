using Edvantix.Blog.Domain.AggregatesModel.PostAggregate;
using Edvantix.Chassis.Exceptions;
using MediatR;

namespace Edvantix.Blog.Features.PostFeature.Features.DeletePost;

/// <summary>
/// Команда для архивирования поста (мягкое удаление через перевод в статус Archived).
/// </summary>
public sealed record DeletePostCommand(long PostId) : IRequest;

/// <summary>
/// Обработчик команды удаления/архивирования поста.
/// Переводит пост в статус Archived вместо физического удаления.
/// </summary>
public sealed class DeletePostCommandHandler(IServiceProvider provider)
    : IRequestHandler<DeletePostCommand>
{
    public async Task Handle(DeletePostCommand request, CancellationToken cancellationToken)
    {
        using var postRepo = provider.GetRequiredService<IPostRepository>();

        var post =
            await postRepo.GetByIdAsync(request.PostId, cancellationToken)
            ?? throw new NotFoundException($"Пост с ID {request.PostId} не найден.");

        post.Archive();

        await postRepo.UpdateAsync(post, cancellationToken);
        await postRepo.SaveEntitiesAsync(cancellationToken);
    }
}
