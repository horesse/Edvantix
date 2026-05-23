namespace Edvantix.Groups.Features.Directories.LessonTypes;

/// <summary>
/// Статистика справочника типов занятий.
/// </summary>
/// <param name="ActiveCount">Количество не архивных записей.</param>
/// <param name="ArchivedCount">Количество архивных записей.</param>
/// <param name="LastModifiedAt">Время последнего изменения любой записи.</param>
public sealed record LessonTypeStats(int ActiveCount, int ArchivedCount, DateTime? LastModifiedAt);

/// <summary>
/// Предоставляет статистику по справочнику типов занятий организации.
/// Используется для отображения сводки на странице справочников и для gRPC-ответов.
/// </summary>
public sealed class LessonTypeStatsProvider(GroupsDbContext context)
{
    /// <summary>Возвращает текущую статистику справочника для указанной организации.</summary>
    public async Task<LessonTypeStats> GetStatsAsync(Guid orgId, CancellationToken ct = default)
    {
        var data = await context
            .LessonTypes.Where(lt => lt.OrganizationId == orgId)
            .GroupBy(_ => 1)
            .Select(g => new
            {
                Active = g.Count(lt => !lt.IsArchived),
                Archived = g.Count(lt => lt.IsArchived),
                LastModified = g.Max(lt => lt.LastModifiedAt),
            })
            .FirstOrDefaultAsync(ct);

        return data is null
            ? new LessonTypeStats(0, 0, null)
            : new LessonTypeStats(data.Active, data.Archived, data.LastModified);
    }
}
