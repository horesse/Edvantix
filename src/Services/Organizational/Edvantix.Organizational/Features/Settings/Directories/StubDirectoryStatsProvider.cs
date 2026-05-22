namespace Edvantix.Organizational.Features.Settings.Directories;

/// <summary>
/// Заглушка для справочников без собственной реализации <see cref="IDirectoryStatsProvider"/>.
/// Возвращает нулевую статистику с <see cref="DirectoryStats.IsAvailable"/> = <c>false</c>.
/// Регистрируется в DI per-code для всех справочников из <see cref="DirectoryCatalog.All"/>,
/// реальные провайдеры (Tasks 4–11) переопределяют заглушку для своего кода, регистрируясь позже.
/// </summary>
public sealed class StubDirectoryStatsProvider(DirectoryDescriptor descriptor)
    : IDirectoryStatsProvider
{
    /// <inheritdoc />
    public DirectoryDescriptor Descriptor => descriptor;

    /// <inheritdoc />
    public Task<DirectoryStats> GetStatsAsync(Guid orgId, CancellationToken ct) =>
        Task.FromResult(
            new DirectoryStats(
                ActiveCount: 0,
                ArchivedCount: 0,
                LastModifiedAt: null,
                IsAvailable: false
            )
        );
}
