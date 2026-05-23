using Edvantix.Groups.Domain.AggregatesModel.SubjectAggregate;

namespace Edvantix.Groups.Features.Directories.Subjects;

/// <summary>
/// Поставщик статистики справочника «Предметы» для указанной организации.
/// <para>Данные возвращаются из локальной БД сервиса Groups.
/// Для агрегации в сервисе Organizational используется gRPC-вызов к этому сервису.</para>
/// </summary>
public sealed class SubjectStatsProvider(ISubjectRepository repository)
{
    /// <summary>Код справочника из <c>DirectoryCatalog</c>.</summary>
    public const string DirectoryCode = "subjects";

    /// <summary>Возвращает текущую статистику справочника для указанной организации.</summary>
    /// <param name="organizationId">Идентификатор организации (тенанта).</param>
    /// <param name="ct">Токен отмены.</param>
    public async Task<(int ActiveCount, int ArchivedCount, DateTime? LastModifiedAt)> GetStatsAsync(
        Guid organizationId,
        CancellationToken ct
    ) => await repository.GetStatsAsync(organizationId, ct);
}
