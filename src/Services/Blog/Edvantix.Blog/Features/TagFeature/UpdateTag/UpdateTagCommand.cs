namespace Edvantix.Blog.Features.TagFeature.UpdateTag;

/// <summary>
/// Команда для обновления тега блога.
/// </summary>
public sealed record UpdateTagCommand(Guid TagId, string Name, string Slug) : IRequest;

public sealed class UpdateTagCommandHandler(IServiceProvider provider)
    : IRequestHandler<UpdateTagCommand>
{
    public async ValueTask<Unit> Handle(
        UpdateTagCommand request,
        CancellationToken cancellationToken
    )
    {
        var tagRepo = provider.GetRequiredService<ITagRepository>();

        var tag =
            await tagRepo.GetByIdAsync(request.TagId, cancellationToken)
            ?? throw new NotFoundException($"Тег с ID {request.TagId} не найден.");

        tag.Update(request.Name, request.Slug);

        await tagRepo.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
