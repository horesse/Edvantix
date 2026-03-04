namespace Edvantix.Catalog.Features.TimezoneFeature.UpdateTimezone;

/// <summary>
/// Команда обновления часового пояса. Требует передачи всех изменяемых полей.
/// <para>
/// Логика IsActive: <c>false</c> → деактивация; <c>true</c> → активация; <c>null</c> → без изменений.
/// </para>
/// </summary>
public sealed record UpdateTimezoneCommand(
    string Code,
    string NameRu,
    string NameEn,
    string DisplayName,
    int UtcOffsetMinutes,
    bool? IsActive
) : IRequest<TimezoneModel>;

/// <summary>
/// Обработчик <see cref="UpdateTimezoneCommand"/>.
/// HTTP 404 — если часовой пояс не найден.
/// </summary>
public sealed class UpdateTimezoneCommandHandler(IServiceProvider provider)
    : IRequestHandler<UpdateTimezoneCommand, TimezoneModel>
{
    /// <inheritdoc/>
    public async ValueTask<TimezoneModel> Handle(
        UpdateTimezoneCommand request,
        CancellationToken cancellationToken
    )
    {
        var repo = provider.GetRequiredService<ITimezoneRepository>();
        var mapper = provider.GetRequiredService<IMapper<Timezone, TimezoneModel>>();

        var timezone =
            await repo.FindTrackedByCodeAsync(request.Code, cancellationToken)
            ?? throw new NotFoundException($"Часовой пояс с кодом '{request.Code}' не найден.");

        if (request.IsActive == false && timezone.IsActive)
        {
            timezone.Deactivate();
        }
        else if (request.IsActive == true && !timezone.IsActive)
        {
            timezone.Activate();
        }

        timezone.Update(
            request.NameRu,
            request.NameEn,
            request.DisplayName,
            request.UtcOffsetMinutes
        );

        await repo.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return mapper.Map(timezone);
    }
}
