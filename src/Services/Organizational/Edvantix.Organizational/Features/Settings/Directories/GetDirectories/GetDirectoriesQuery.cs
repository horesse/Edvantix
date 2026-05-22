using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.Permissions;
using ZiggyCreatures.Caching.Fusion;

namespace Edvantix.Organizational.Features.Settings.Directories.GetDirectories;

/// <summary>
/// Возвращает каталог справочников настроек — ровно 8 элементов в фиксированном порядке
/// из <see cref="DirectoryCatalog.All"/>, дополненных статистикой по организации.
/// Отсутствующие провайдеры дают <c>IsAvailable = false</c>.
/// </summary>
[RequirePermission(OrganizationPermissions.View)]
public sealed record GetDirectoriesQuery : IQuery<IReadOnlyList<DirectorySummaryDto>>;

internal sealed class GetDirectoriesQueryHandler(
    ITenantContext tenantContext,
    IEnumerable<IDirectoryStatsProvider> providers,
    IFusionCache cache,
    ILogger<GetDirectoriesQueryHandler> logger
) : IQueryHandler<GetDirectoriesQuery, IReadOnlyList<DirectorySummaryDto>>
{
    private static readonly DirectoryStats UnavailableStats = new(
        ActiveCount: 0,
        ArchivedCount: 0,
        LastModifiedAt: null,
        IsAvailable: false
    );

    public async ValueTask<IReadOnlyList<DirectorySummaryDto>> Handle(
        GetDirectoriesQuery query,
        CancellationToken cancellationToken
    )
    {
        var orgId = tenantContext.OrganizationId;
        var cacheKey = $"org:{orgId}:dir-stats";

        return await cache.GetOrSetAsync(
            cacheKey,
            async ct => await FetchAllAsync(orgId, ct),
            options => options.SetDuration(TimeSpan.FromSeconds(60)),
            token: cancellationToken
        );
    }

    private async Task<IReadOnlyList<DirectorySummaryDto>> FetchAllAsync(
        Guid orgId,
        CancellationToken ct
    )
    {
        // Last registration wins: реальные провайдеры переопределяют заглушки
        var providerMap = new Dictionary<string, IDirectoryStatsProvider>(StringComparer.Ordinal);
        foreach (var provider in providers)
        {
            providerMap[provider.Descriptor.Code] = provider;
        }

        var tasks = DirectoryCatalog.All.Select(descriptor =>
            FetchOneAsync(descriptor, providerMap, orgId, ct)
        );

        return await Task.WhenAll(tasks);
    }

    private async Task<DirectorySummaryDto> FetchOneAsync(
        DirectoryDescriptor descriptor,
        IReadOnlyDictionary<string, IDirectoryStatsProvider> providerMap,
        Guid orgId,
        CancellationToken ct
    )
    {
        if (!providerMap.TryGetValue(descriptor.Code, out var provider))
        {
            logger.LogWarning(
                "Провайдер статистики для справочника '{Code}' не зарегистрирован",
                descriptor.Code
            );
            return DirectorySummaryDto.From(descriptor, UnavailableStats);
        }

        try
        {
            var stats = await provider.GetStatsAsync(orgId, ct);
            return DirectorySummaryDto.From(descriptor, stats);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogWarning(
                ex,
                "Провайдер статистики для справочника '{Code}' завершился с ошибкой",
                descriptor.Code
            );
            return DirectorySummaryDto.From(descriptor, UnavailableStats);
        }
    }
}
