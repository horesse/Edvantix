using Edvantix.Organizational.Features.Settings.Directories;
using Edvantix.Organizational.Features.Settings.Directories.GetDirectories;

namespace Edvantix.Organizational.UnitTests.Features.Settings.Directories.GetDirectories;

/// <summary>
/// Snapshot-тесты фиксируют форму <see cref="DirectorySummaryDto"/>.
/// Любое изменение состава полей или типов требует явного переутверждения snapshot-файла.
/// </summary>
public sealed class DirectorySummaryDtoSnapshotTests
{
    [Test]
    public Task DirectorySummaryDto_UnavailableShape_ShouldMatchSnapshot()
    {
        var dto = DirectorySummaryDto.From(
            DirectoryCatalog.All[0],
            new DirectoryStats(
                ActiveCount: 0,
                ArchivedCount: 0,
                LastModifiedAt: null,
                IsAvailable: false
            )
        );

        return Verify(dto);
    }

    [Test]
    public Task DirectorySummaryDto_FullShape_ShouldMatchSnapshot()
    {
        var dto = DirectorySummaryDto.From(
            DirectoryCatalog.All[0],
            new DirectoryStats(
                ActiveCount: 12,
                ArchivedCount: 3,
                LastModifiedAt: new DateTimeOffset(2026, 5, 21, 12, 0, 0, TimeSpan.Zero),
                IsAvailable: true
            )
        );

        return Verify(dto);
    }

    [Test]
    public Task DirectorySummaryDto_WithBadge_ShouldMatchSnapshot()
    {
        // student-statuses — единственный справочник с бейджем «системный»
        var descriptor = DirectoryCatalog.FindByCode(DirectoryCatalog.StudentStatuses)!;
        var dto = DirectorySummaryDto.From(
            descriptor,
            new DirectoryStats(
                ActiveCount: 4,
                ArchivedCount: 0,
                LastModifiedAt: null,
                IsAvailable: true
            )
        );

        return Verify(dto);
    }

    [Test]
    public Task DirectorySummaryDtoArray_AllStubs_ShouldMatchSnapshot()
    {
        var dtos = DirectoryCatalog
            .All.Select(d =>
                DirectorySummaryDto.From(d, new DirectoryStats(0, 0, null, IsAvailable: false))
            )
            .ToList();

        return Verify(dtos);
    }
}
