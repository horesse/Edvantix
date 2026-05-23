using Edvantix.Organizational.Features.Roles.Summary;

namespace Edvantix.Organizational.UnitTests.Features.Roles.Summary;

/// <summary>
/// Snapshot-тест фиксирует форму JSON <see cref="RolesSummaryDto"/>.
/// Любое изменение состава полей или их типов требует явного переутверждения snapshot-файла.
/// </summary>
public sealed class RolesSummaryDtoSnapshotTests
{
    [Test]
    public Task RolesSummaryDto_FullShape_ShouldMatchSnapshot()
    {
        var dto = new RolesSummaryDto(
            TotalRoles: 7,
            AssignedMembersCount: 5,
            RoleNamesPreview: ["Владелец", "Директор", "Преподаватель", "Администратор", "Методист"]
        );

        return Verify(dto);
    }

    [Test]
    public Task RolesSummaryDto_EmptyOrganization_ShouldMatchSnapshot()
    {
        var dto = new RolesSummaryDto(TotalRoles: 0, AssignedMembersCount: 0, RoleNamesPreview: []);

        return Verify(dto);
    }
}
