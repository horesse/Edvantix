namespace Edvantix.Organizational.UnitTests.Domain;

public sealed class OrganizationAggregateTests
{
    private static readonly Guid ValidCountryId = Guid.CreateVersion7();
    private static readonly Guid ValidCurrencyId = Guid.CreateVersion7();

    private static Organization CreateValidOrganization(string? shortName = null) =>
        new(
            "ООО Тестовая Компания",
            isLegalEntity: true,
            new DateOnly(2020, 1, 15),
            LegalForm.Llc,
            ValidCountryId,
            ValidCurrencyId,
            OrganizationType.PrivateEducationalCenter,
            shortName
        );

    [Test]
    public void GivenValidData_WhenCreatingOrganization_ThenShouldInitializePropertiesCorrectly()
    {
        var registrationDate = new DateOnly(2020, 1, 15);

        var org = new Organization(
            "ООО Тестовая Компания",
            isLegalEntity: true,
            registrationDate,
            LegalForm.Llc,
            ValidCountryId,
            ValidCurrencyId,
            OrganizationType.PrivateEducationalCenter,
            "ТестКо"
        );

        org.FullLegalName.ShouldBe("ООО Тестовая Компания");
        org.ShortName.ShouldBe("ТестКо");
        org.IsLegalEntity.ShouldBeTrue();
        org.RegistrationDate.ShouldBe(registrationDate);
        org.LegalForm.ShouldBe(LegalForm.Llc);
        org.CountryId.ShouldBe(ValidCountryId);
        org.CurrencyId.ShouldBe(ValidCurrencyId);
        org.OrganizationType.ShouldBe(OrganizationType.PrivateEducationalCenter);
        org.Status.ShouldBe(OrganizationStatus.Active);
        org.IsDeleted.ShouldBeFalse();
        org.Contacts.ShouldBeEmpty();
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("   ")]
    public void GivenNullOrWhiteSpaceFullLegalName_WhenCreatingOrganization_ThenShouldThrowArgumentException(
        string? fullLegalName
    )
    {
        var act = () =>
            new Organization(
                fullLegalName!,
                isLegalEntity: true,
                new DateOnly(2020, 1, 1),
                LegalForm.Llc,
                ValidCountryId,
                ValidCurrencyId,
                OrganizationType.PrivateEducationalCenter
            );

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenEmptyCountryId_WhenCreatingOrganization_ThenShouldThrowArgumentException()
    {
        var act = () =>
            new Organization(
                "ООО Тестовая Компания",
                isLegalEntity: true,
                new DateOnly(2020, 1, 1),
                LegalForm.Llc,
                Guid.Empty,
                ValidCurrencyId,
                OrganizationType.PrivateEducationalCenter
            );

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenEmptyCurrencyId_WhenCreatingOrganization_ThenShouldThrowArgumentException()
    {
        var act = () =>
            new Organization(
                "ООО Тестовая Компания",
                isLegalEntity: true,
                new DateOnly(2020, 1, 1),
                LegalForm.Llc,
                ValidCountryId,
                Guid.Empty,
                OrganizationType.PrivateEducationalCenter
            );

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenValidData_WhenUpdating_ThenShouldUpdateProperties()
    {
        var org = CreateValidOrganization();

        var newDate = new DateOnly(2022, 6, 1);
        org.Update(
            "АО Новое Название",
            "НовНаз",
            OrganizationType.University,
            LegalForm.Ojsc,
            newDate
        );

        org.FullLegalName.ShouldBe("АО Новое Название");
        org.ShortName.ShouldBe("НовНаз");
        org.OrganizationType.ShouldBe(OrganizationType.University);
        org.LegalForm.ShouldBe(LegalForm.Ojsc);
        org.RegistrationDate.ShouldBe(newDate);
    }

    [Test]
    public void GivenValidData_WhenUpdating_ThenShouldRegisterOrganizationUpdatedDomainEvent()
    {
        var org = CreateValidOrganization();

        org.Update(
            "АО Новое Название",
            "НовНаз",
            OrganizationType.University,
            LegalForm.Ojsc,
            new DateOnly(2022, 6, 1)
        );

        org.DomainEvents.ShouldHaveSingleItem();
        var @event = org.DomainEvents.Single().ShouldBeOfType<OrganizationUpdatedDomainEvent>();
        @event.OrganizationId.ShouldBe(org.Id);
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("   ")]
    public void GivenNullOrWhiteSpaceFullLegalName_WhenUpdating_ThenShouldThrowArgumentException(
        string? fullLegalName
    )
    {
        var org = CreateValidOrganization();

        var act = () =>
            org.Update(
                fullLegalName!,
                null,
                OrganizationType.PrivateEducationalCenter,
                LegalForm.Llc,
                new DateOnly(2020, 1, 15)
            );

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenActiveOrganization_WhenArchiving_ThenStatusShouldBeArchived()
    {
        var org = CreateValidOrganization();

        org.Archive();

        org.Status.ShouldBe(OrganizationStatus.Archived);
        org.IsDeleted.ShouldBeFalse();
    }

    [Test]
    public void GivenActiveOrganization_WhenDeleting_ThenShouldMarkAsDeletedAndSetDeletedStatus()
    {
        var org = CreateValidOrganization();

        org.Delete();

        org.IsDeleted.ShouldBeTrue();
        org.Status.ShouldBe(OrganizationStatus.Deleted);
    }

    [Test]
    public void GivenActiveOrganization_WhenDeleting_ThenShouldRegisterOrganizationDeletedDomainEvent()
    {
        var org = CreateValidOrganization();

        org.Delete();

        org.DomainEvents.ShouldHaveSingleItem();
        var @event = org.DomainEvents.Single().ShouldBeOfType<OrganizationDeletedDomainEvent>();
        @event.OrganizationId.ShouldBe(org.Id);
    }

    [Test]
    public void GivenNonPrimaryContact_WhenAddingContact_ThenShouldAddToContacts()
    {
        var org = CreateValidOrganization();
        var contact = new Contact(
            Guid.CreateVersion7(),
            "info@example.com",
            "Основной email",
            ContactType.Email,
            isPrimary: false
        );

        org.AddContact(contact);

        org.Contacts.ShouldHaveSingleItem();
        org.Contacts[0].ShouldBe(contact);
    }

    [Test]
    public void GivenExistingPrimaryContact_WhenAddingNewPrimaryContact_ThenPreviousPrimaryContactShouldBeUnset()
    {
        var orgId = Guid.CreateVersion7();
        var org = CreateValidOrganization();
        var firstContact = new Contact(
            orgId,
            "first@example.com",
            "Первый",
            ContactType.Email,
            isPrimary: true
        );
        org.AddContact(firstContact);

        var secondContact = new Contact(
            orgId,
            "second@example.com",
            "Второй",
            ContactType.Email,
            isPrimary: true
        );
        org.AddContact(secondContact);

        org.Contacts.Count.ShouldBe(2);
        firstContact.IsPrimary.ShouldBeFalse();
        secondContact.IsPrimary.ShouldBeTrue();
    }

    [Test]
    public void GivenExistingContact_WhenSettingPrimaryContact_ThenShouldSetPrimaryAndUnsetOthers()
    {
        var orgId = Guid.CreateVersion7();
        var org = CreateValidOrganization();
        var firstContact = new Contact(
            orgId,
            "first@example.com",
            "Первый",
            ContactType.Email,
            isPrimary: true
        );
        firstContact.Id = Guid.CreateVersion7();
        var secondContact = new Contact(
            orgId,
            "+79001234567",
            "Второй",
            ContactType.MobilePhone,
            isPrimary: false
        );
        secondContact.Id = Guid.CreateVersion7();
        org.AddContact(firstContact);
        org.AddContact(secondContact);

        org.SetPrimaryContact(secondContact.Id);

        firstContact.IsPrimary.ShouldBeFalse();
        secondContact.IsPrimary.ShouldBeTrue();
    }

    [Test]
    public void GivenNonExistentContactId_WhenSettingPrimaryContact_ThenShouldThrowInvalidOperationException()
    {
        var org = CreateValidOrganization();

        var act = () => org.SetPrimaryContact(Guid.CreateVersion7());

        act.ShouldThrow<InvalidOperationException>();
    }

    [Test]
    public void GivenFullLegalNameWithLeadingSpaces_WhenCreatingOrganization_ThenShouldTrimName()
    {
        var org = new Organization(
            "  ООО Тестовая Компания  ",
            isLegalEntity: true,
            new DateOnly(2020, 1, 1),
            LegalForm.Llc,
            ValidCountryId,
            ValidCurrencyId,
            OrganizationType.PrivateEducationalCenter
        );

        org.FullLegalName.ShouldBe("ООО Тестовая Компания");
    }

    [Test]
    public void GivenNullShortName_WhenCreatingOrganization_ThenShortNameShouldBeNull()
    {
        var org = CreateValidOrganization(shortName: null);

        org.ShortName.ShouldBeNull();
    }

    [Test]
    public void GivenValidOwnerProfileId_WhenInitializingOwnership_ThenShouldRegisterDomainEvent()
    {
        var org = CreateValidOrganization();
        var ownerProfileId = Guid.CreateVersion7();

        org.InitializeOwnership(ownerProfileId);

        org.DomainEvents.ShouldHaveSingleItem();
        var @event = org.DomainEvents.Single().ShouldBeOfType<OrganizationCreatedDomainEvent>();
        @event.OrganizationId.ShouldBe(org.Id);
        @event.OwnerProfileId.ShouldBe(ownerProfileId);
    }

    [Test]
    public void GivenEmptyOwnerProfileId_WhenInitializingOwnership_ThenShouldThrowArgumentException()
    {
        var org = CreateValidOrganization();

        var act = () => org.InitializeOwnership(Guid.Empty);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenValidOwnerProfileId_WhenInitializingOwnershipTwice_ThenShouldHaveTwoDomainEvents()
    {
        var org = CreateValidOrganization();

        org.InitializeOwnership(Guid.CreateVersion7());
        org.InitializeOwnership(Guid.CreateVersion7());

        org.DomainEvents.Count.ShouldBe(2);
    }
}
