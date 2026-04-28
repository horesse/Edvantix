namespace Edvantix.Organizational.UnitTests.Features.Organizations;

public sealed class OrganizationDetailDtoMapperTests
{
    private readonly OrganizationDetailDtoMapper _mapper = new();

    private static readonly Guid CountryId = Guid.CreateVersion7();
    private static readonly Guid CurrencyId = Guid.CreateVersion7();

    [Test]
    public void GivenOrganizationWithoutContacts_WhenMapping_ThenShouldMapAllProperties()
    {
        var registrationDate = new DateOnly(2021, 6, 15);
        var org = new Organization(
            "ООО Маппер Тест",
            isLegalEntity: true,
            registrationDate,
            LegalForm.Pue,
            CountryId,
            CurrencyId,
            OrganizationType.ItSchool,
            "МТ"
        );

        var dto = _mapper.Map(org);

        dto.Id.ShouldBe(org.Id);
        dto.FullLegalName.ShouldBe("ООО Маппер Тест");
        dto.ShortName.ShouldBe("МТ");
        dto.IsLegalEntity.ShouldBeTrue();
        dto.RegistrationDate.ShouldBe(registrationDate);
        dto.LegalForm.ShouldBe(LegalForm.Pue);
        dto.CountryId.ShouldBe(CountryId);
        dto.CurrencyId.ShouldBe(CurrencyId);
        dto.OrganizationType.ShouldBe(OrganizationType.ItSchool);
        dto.Status.ShouldBe(OrganizationStatus.Active);
        dto.Contacts.ShouldBeEmpty();
        dto.LastModifiedAt.ShouldBeNull();
    }

    [Test]
    public void GivenOrganizationWithActiveContacts_WhenMapping_ThenShouldIncludeContacts()
    {
        var org = CreateOrganization();
        var contact = new Contact(
            org.Id,
            "info@test.com",
            "Основной",
            ContactType.Email,
            isPrimary: true
        );
        org.AddContact(contact);

        var dto = _mapper.Map(org);

        dto.Contacts.ShouldHaveSingleItem();
        dto.Contacts[0].Value.ShouldBe("info@test.com");
        dto.Contacts[0].IsPrimary.ShouldBeTrue();
        dto.Contacts[0].ContactType.ShouldBe(ContactType.Email);
    }

    [Test]
    public void GivenOrganizationWithNullShortName_WhenMapping_ThenShortNameShouldBeNull()
    {
        var org = new Organization(
            "ООО Без Краткого",
            isLegalEntity: false,
            new DateOnly(2019, 3, 1),
            LegalForm.IndividualEntrepreneur,
            CountryId,
            CurrencyId,
            OrganizationType.TutoringCenter
        );

        var dto = _mapper.Map(org);

        dto.ShortName.ShouldBeNull();
    }

    private static Organization CreateOrganization() =>
        new(
            "ООО Тест",
            isLegalEntity: true,
            new DateOnly(2020, 1, 1),
            LegalForm.Llc,
            CountryId,
            CurrencyId,
            OrganizationType.PrivateEducationalCenter
        );
}
