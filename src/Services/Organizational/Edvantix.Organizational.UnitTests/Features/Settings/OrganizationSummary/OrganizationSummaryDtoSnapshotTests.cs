using Edvantix.Organizational.Features.Organizations;
using Edvantix.Organizational.Features.Settings.OrganizationSummary;

namespace Edvantix.Organizational.UnitTests.Features.Settings.OrganizationSummary;

/// <summary>
/// Snapshot-тест фиксирует форму JSON <see cref="OrganizationSummaryDto"/>.
/// Любое изменение состава полей или их типов требует явного переутверждения snapshot-файла.
/// </summary>
public sealed class OrganizationSummaryDtoSnapshotTests
{
    [Test]
    public Task OrganizationSummaryDto_FullShape_ShouldMatchSnapshot()
    {
        var dto = new OrganizationSummaryDto(
            Id: new Guid("00000000-0000-7000-8000-000000000001"),
            FullLegalName: "ООО Образовательный центр «Знание»",
            ShortName: "ОЦ Знание",
            OrganizationType: OrganizationType.PrivateEducationalCenter,
            Status: OrganizationStatus.Active,
            IsLegalEntity: true,
            MembersCount: 25,
            PrimaryContact: new ContactDto(
                Id: new Guid("00000000-0000-7000-8000-000000000002"),
                Value: "info@znanie.ru",
                Description: "Основной email",
                ContactType: ContactType.Email,
                IsPrimary: true
            ),
            LastModified: new OrganizationSummaryDto.LastModifiedInfo(
                At: new DateTime(2026, 5, 21, 10, 0, 0, DateTimeKind.Utc),
                ByDisplayName: "Иван Иванов"
            )
        );

        return Verify(dto);
    }

    [Test]
    public Task OrganizationSummaryDto_WithNullOptionals_ShouldMatchSnapshot()
    {
        var dto = new OrganizationSummaryDto(
            Id: new Guid("00000000-0000-7000-8000-000000000003"),
            FullLegalName: "ИП Петров",
            ShortName: null,
            OrganizationType: OrganizationType.IndividualEntrepreneur,
            Status: OrganizationStatus.Active,
            IsLegalEntity: false,
            MembersCount: 0,
            PrimaryContact: null,
            LastModified: new OrganizationSummaryDto.LastModifiedInfo(At: null, ByDisplayName: null)
        );

        return Verify(dto);
    }
}
