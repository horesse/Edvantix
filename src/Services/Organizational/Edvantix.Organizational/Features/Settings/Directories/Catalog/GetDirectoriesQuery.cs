using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.Permissions;
using ZiggyCreatures.Caching.Fusion;

namespace Edvantix.Organizational.Features.Settings.Directories.Catalog;

/// <summary>
/// Возвращает каталог всех 8 справочников организации с актуальной статистикой.
/// Порядок элементов фиксирован и совпадает с <see cref="DirectoryCatalog.All"/>.
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
    public async ValueTask<IReadOnlyList<DirectorySummaryDto>> Handle(
        GetDirectoriesQuery query,
        CancellationToken cancellationToken
    )
    {
        var orgId = tenantContext.OrganizationId;
        var cacheKey = $"org:{orgId}:dir-stats";

        return await cache.GetOrSetAsync(
            cacheKey,
            async ct => await FetchDirectoriesAsync(orgId, ct),
            options => options.SetDuration(TimeSpan.FromSeconds(60)),
            token: cancellationToken
        );
    }

    private async Task<IReadOnlyList<DirectorySummaryDto>> FetchDirectoriesAsync(
        Guid orgId,
        CancellationToken ct
    )
    {
        var providersByCode = providers.ToDictionary(
            p => p.Descriptor.Code,
            StringComparer.Ordinal
        );

        var summaries = new List<DirectorySummaryDto>(DirectoryCatalog.All.Count);

        foreach (var descriptor in DirectoryCatalog.All)
        {
            var provider =
                providersByCode.GetValueOrDefault(descriptor.Code)
                ?? new StubDirectoryStatsProvider(descriptor);

            var stats = await FetchWithFallbackAsync(provider, orgId, ct);

            summaries.Add(
                new DirectorySummaryDto(
                    descriptor.Code,
                    descriptor.Name,
                    descriptor.Description,
                    descriptor.Icon,
                    descriptor.Badge,
                    stats.ActiveCount,
                    stats.ArchivedCount,
                    stats.LastModifiedAt,
                    stats.IsAvailable
                )
            );
        }

        return summaries;
    }

    private async Task<DirectoryStats> FetchWithFallbackAsync(
        IDirectoryStatsProvider provider,
        Guid orgId,
        CancellationToken ct
    )
    {
        try
        {
            return await provider.GetStatsAsync(orgId, ct);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            // Глобальная отмена запроса — пробрасываем выше.
            throw;
        }
        catch (Exception ex)
        {
            logger.LogWarning(
                ex,
                "Не удалось получить статистику справочника {Code}. Возвращаем заглушку.",
                provider.Descriptor.Code
            );
            return new DirectoryStats(0, 0, null, false);
        }
    }
}
