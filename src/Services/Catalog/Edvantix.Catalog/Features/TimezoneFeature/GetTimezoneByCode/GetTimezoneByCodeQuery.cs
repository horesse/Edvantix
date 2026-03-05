using Edvantix.Catalog.Application.Behaviors;

namespace Edvantix.Catalog.Features.TimezoneFeature.GetTimezoneByCode;

/// <summary>
/// Запрос часового пояса по IANA-коду. Реализует <see cref="ICachedQuery"/> — результат кэшируется через HybridCache.
/// </summary>
public sealed record GetTimezoneByCodeQuery(string Code) : IRequest<TimezoneModel>, ICachedQuery
{
    /// <inheritdoc/>
    public string CacheKey => $"catalog:timezone:{Code}";

    /// <inheritdoc/>
    public string[] Tags => [CatalogEntityType.Timezone];

    /// <inheritdoc/>
    public TimeSpan? Expiry => TimeSpan.FromHours(4);
}

/// <summary>
/// Обработчик <see cref="GetTimezoneByCodeQuery"/>.
/// Выбрасывает <see cref="NotFoundException"/>, если часовой пояс не найден.
/// </summary>
public sealed class GetTimezoneByCodeQueryHandler(IServiceProvider provider)
    : IRequestHandler<GetTimezoneByCodeQuery, TimezoneModel>
{
    /// <inheritdoc/>
    public async ValueTask<TimezoneModel> Handle(
        GetTimezoneByCodeQuery request,
        CancellationToken cancellationToken
    )
    {
        var repo = provider.GetRequiredService<ITimezoneRepository>();
        var mapper = provider.GetRequiredService<IMapper<Timezone, TimezoneModel>>();

        var timezone =
            await repo.GetByCodeAsync(request.Code, cancellationToken)
            ?? throw new NotFoundException($"Часовой пояс с кодом '{request.Code}' не найден.");

        return mapper.Map(timezone);
    }
}
