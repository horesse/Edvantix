using Edvantix.Organizational.Grpc.Services.Groups;

namespace Edvantix.Organizational.Features.Settings.Directories.Levels;

/// <summary>
/// Провайдер статистики справочника «Уровни».
/// <para>Данные получаются из Groups-сервиса через gRPC.</para>
/// </summary>
public sealed class LevelDirectoryStatsProvider(IGroupsService groupsService)
    : IDirectoryStatsProvider
{
    public DirectoryDescriptor Descriptor => DirectoryCatalog.FindByCode(DirectoryCatalog.Levels)!;

    public async Task<DirectoryStats> GetStatsAsync(Guid orgId, CancellationToken ct)
    {
        try
        {
            var (active, archived) = await groupsService.GetLevelStatsAsync(orgId, ct);

            return new DirectoryStats(
                ActiveCount: active,
                ArchivedCount: archived,
                LastModifiedAt: null,
                IsAvailable: true
            );
        }
        catch
        {
            // Groups-сервис недоступен — возвращаем заглушку, не ломаем страницу настроек.
            return new DirectoryStats(0, 0, null, IsAvailable: false);
        }
    }
}
