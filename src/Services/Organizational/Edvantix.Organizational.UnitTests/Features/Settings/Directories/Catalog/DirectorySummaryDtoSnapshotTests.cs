using Edvantix.Organizational.Features.Settings.Directories;
using Edvantix.Organizational.Features.Settings.Directories.Catalog;

namespace Edvantix.Organizational.UnitTests.Features.Settings.Directories.Catalog;

/// <summary>
/// Snapshot-тест фиксирует форму JSON <see cref="DirectorySummaryDto"/>.
/// Любое изменение полей DTO или порядка/состава <see cref="DirectoryCatalog.All"/>
/// требует явного переутверждения snapshot-файла.
/// </summary>
public sealed class DirectorySummaryDtoSnapshotTests
{
    [Test]
    public Task DirectorySummaryDto_AllStubs_ShouldMatchSnapshot()
    {
        var items = DirectoryCatalog
            .All.Select(d => new DirectorySummaryDto(
                d.Code,
                d.Name,
                d.Description,
                d.Icon,
                d.Badge,
                ActiveCount: 0,
                ArchivedCount: 0,
                LastModifiedAt: null,
                IsAvailable: false
            ))
            .ToList();

        return Verify(items);
    }

    [Test]
    public Task DirectorySummaryDto_WithStats_ShouldMatchSnapshot()
    {
        var item = new DirectorySummaryDto(
            Code: DirectoryCatalog.Levels,
            Name: "Уровни",
            Description: "Уровни обучения для групп и курсов.",
            Icon: "Layers",
            Badge: null,
            ActiveCount: 12,
            ArchivedCount: 3,
            LastModifiedAt: new DateTimeOffset(2026, 5, 22, 10, 0, 0, TimeSpan.Zero),
            IsAvailable: true
        );

        return Verify(item);
    }
}
