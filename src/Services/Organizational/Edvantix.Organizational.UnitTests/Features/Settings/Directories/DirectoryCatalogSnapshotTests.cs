using Edvantix.Organizational.Features.Settings.Directories;

namespace Edvantix.Organizational.UnitTests.Features.Settings.Directories;

/// <summary>
/// Snapshot-тесты на схему базовых DTO каталога справочников.
/// Любое изменение состава полей <see cref="DirectoryDescriptor"/>/<see cref="DirectoryStats"/>
/// или порядка элементов в <see cref="DirectoryCatalog.All"/> приведёт к фейлу теста —
/// требуется явное переутверждение snapshot-файла.
/// </summary>
public sealed class DirectoryCatalogSnapshotTests
{
    [Test]
    public Task DirectoryCatalogAll_ShouldMatchSnapshot() => Verify(DirectoryCatalog.All);

    [Test]
    public Task DirectoryStats_DefaultShape_ShouldMatchSnapshot()
    {
        var sample = new DirectoryStats(
            ActiveCount: 7,
            ArchivedCount: 1,
            LastModifiedAt: new DateTimeOffset(2026, 5, 21, 12, 0, 0, TimeSpan.Zero),
            IsAvailable: true
        );

        return Verify(sample);
    }
}
