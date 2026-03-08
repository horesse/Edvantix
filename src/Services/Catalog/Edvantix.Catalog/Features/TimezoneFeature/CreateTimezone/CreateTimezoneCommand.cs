namespace Edvantix.Catalog.Features.TimezoneFeature.CreateTimezone;

using Mediator;

/// <summary>
/// Команда создания часового пояса.
/// </summary>
public sealed record CreateTimezoneCommand(
    string Code,
    string NameRu,
    string NameEn,
    string DisplayName,
    int UtcOffsetMinutes
) : ICommand<TimezoneModel>;

/// <summary>
/// Обработчик <see cref="CreateTimezoneCommand"/>.
/// HTTP 409 — если часовой пояс с таким кодом уже существует.
/// </summary>
public sealed class CreateTimezoneCommandHandler(IServiceProvider provider)
    : ICommandHandler<CreateTimezoneCommand, TimezoneModel>
{
    /// <inheritdoc/>
    public async ValueTask<TimezoneModel> Handle(
        CreateTimezoneCommand request,
        CancellationToken cancellationToken
    )
    {
        var repo = provider.GetRequiredService<ITimezoneRepository>();
        var mapper = provider.GetRequiredService<IMapper<Timezone, TimezoneModel>>();

        var existing = await repo.GetByCodeAsync(request.Code, cancellationToken);
        if (existing is not null)
        {
            throw new InvalidOperationException(
                $"Часовой пояс с кодом '{request.Code}' уже существует."
            );
        }

        var timezone = new Timezone(
            request.Code,
            request.NameRu,
            request.NameEn,
            request.DisplayName,
            request.UtcOffsetMinutes
        );

        await repo.AddAsync(timezone, cancellationToken);
        await repo.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return mapper.Map(timezone);
    }
}
