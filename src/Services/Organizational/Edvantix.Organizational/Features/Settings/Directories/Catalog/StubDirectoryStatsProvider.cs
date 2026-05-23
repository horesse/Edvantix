namespace Edvantix.Organizational.Features.Settings.Directories.Catalog;

/// <summary>
/// Заглушка-провайдер для справочника, не имеющего реализации.
/// Всегда возвращает статистику с <see cref="DirectoryStats.IsAvailable"/> = <c>false</c>,
/// чтобы фронтенд мог отобразить карточку в неактивном состоянии.
/// </summary>
internal sealed class StubDirectoryStatsProvider(DirectoryDescriptor descriptor)
    : IDirectoryStatsProvider
{
    public DirectoryDescriptor Descriptor => descriptor;

    public Task<DirectoryStats> GetStatsAsync(Guid orgId, CancellationToken ct) =>
        Task.FromResult(new DirectoryStats(0, 0, null, false));
}
