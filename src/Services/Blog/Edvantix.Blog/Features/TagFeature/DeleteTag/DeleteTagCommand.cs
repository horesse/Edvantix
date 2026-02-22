namespace Edvantix.Blog.Features.TagFeature.DeleteTag;

/// <summary>
/// Команда для удаления тега блога.
/// </summary>
public sealed record DeleteTagCommand(ulong TagId) : IRequest;

public sealed class DeleteTagCommandHandler(IServiceProvider provider)
    : IRequestHandler<DeleteTagCommand>
{
    public async ValueTask<Unit> Handle(
        DeleteTagCommand request,
        CancellationToken cancellationToken
    )
    {
        var tagRepo = provider.GetRequiredService<ITagRepository>();

        var tag =
            await tagRepo.GetByIdAsync(request.TagId, cancellationToken)
            ?? throw new NotFoundException($"Тег с ID {request.TagId} не найден.");

        await tagRepo.DeleteAsync(tag, cancellationToken);
        await tagRepo.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
