namespace Edvantix.Organizational.UnitTests.Features.Organizations;

public sealed class OrganizationDtoMapperTests
{
    private readonly OrganizationDtoMapper _mapper = new();

    private static readonly Guid CountryId = Guid.CreateVersion7();
    private static readonly Guid CurrencyId = Guid.CreateVersion7();

    [Test]
    public void GivenOrganizationWithShortName_WhenMapping_ThenShouldMapAllProperties()
    {
        var org = new Organization(
            "ООО Тест Маппер",
            isLegalEntity: true,
            new DateOnly(2021, 3, 10),
            LegalForm.Llc,
            CountryId,
            CurrencyId,
            OrganizationType.ItSchool,
            "ТМ"
        );

        var dto = _mapper.Map(org);

        dto.Id.ShouldBe(org.Id);
        dto.FullLegalName.ShouldBe("ООО Тест Маппер");
        dto.ShortName.ShouldBe("ТМ");
        dto.OrganizationType.ShouldBe(OrganizationType.ItSchool);
        dto.Status.ShouldBe(OrganizationStatus.Active);
        dto.IsLegalEntity.ShouldBeTrue();
    }

    [Test]
    public void GivenOrganizationWithoutShortName_WhenMapping_ThenShortNameShouldBeNull()
    {
        var org = new Organization(
            "ООО Без Краткого",
            isLegalEntity: false,
            new DateOnly(2019, 5, 1),
            LegalForm.IndividualEntrepreneur,
            CountryId,
            CurrencyId,
            OrganizationType.TutoringCenter
        );

        var dto = _mapper.Map(org);

        dto.ShortName.ShouldBeNull();
        dto.IsLegalEntity.ShouldBeFalse();
        dto.OrganizationType.ShouldBe(OrganizationType.TutoringCenter);
    }

    [Test]
    public void GivenArchivedOrganization_WhenMapping_ThenStatusShouldBeArchived()
    {
        var org = new Organization(
            "ООО Архив",
            isLegalEntity: true,
            new DateOnly(2015, 1, 1),
            LegalForm.Llc,
            CountryId,
            CurrencyId,
            OrganizationType.PrivateEducationalCenter
        );
        org.Archive();

        var dto = _mapper.Map(org);

        dto.Status.ShouldBe(OrganizationStatus.Archived);
    }
}
