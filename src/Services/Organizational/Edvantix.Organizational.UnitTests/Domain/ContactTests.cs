namespace Edvantix.Organizational.UnitTests.Domain;

public sealed class ContactTests
{
    private static readonly Guid ValidOrgId = Guid.CreateVersion7();

    [Test]
    public void GivenValidData_WhenCreatingContact_ThenShouldInitializePropertiesCorrectly()
    {
        var contact = new Contact(
            ValidOrgId,
            "info@example.com",
            "Основной email",
            ContactType.Email,
            isPrimary: true
        );

        contact.OrganizationId.ShouldBe(ValidOrgId);
        contact.Value.ShouldBe("info@example.com");
        contact.Description.ShouldBe("Основной email");
        contact.ContactType.ShouldBe(ContactType.Email);
        contact.IsPrimary.ShouldBeTrue();
    }

    [Test]
    public void GivenEmptyOrganizationId_WhenCreatingContact_ThenShouldThrowArgumentException()
    {
        var act = () => new Contact(Guid.Empty, "info@example.com", "Описание", ContactType.Email);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("   ")]
    public void GivenNullOrWhiteSpaceValue_WhenCreatingContact_ThenShouldThrowArgumentException(
        string? value
    )
    {
        var act = () => new Contact(ValidOrgId, value!, "Описание", ContactType.Email);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("   ")]
    public void GivenNullOrWhiteSpaceDescription_WhenCreatingContact_ThenShouldThrowArgumentException(
        string? description
    )
    {
        var act = () =>
            new Contact(ValidOrgId, "info@example.com", description!, ContactType.Email);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenNonPrimaryContact_WhenSettingPrimary_ThenIsPrimaryShouldBeTrue()
    {
        var contact = new Contact(
            ValidOrgId,
            "info@example.com",
            "Описание",
            ContactType.Email,
            isPrimary: false
        );

        contact.SetPrimary();

        contact.IsPrimary.ShouldBeTrue();
    }

    [Test]
    public void GivenPrimaryContact_WhenUnsettingPrimary_ThenIsPrimaryShouldBeFalse()
    {
        var contact = new Contact(
            ValidOrgId,
            "info@example.com",
            "Описание",
            ContactType.Email,
            isPrimary: true
        );

        contact.UnsetPrimary();

        contact.IsPrimary.ShouldBeFalse();
    }

    [Test]
    public void GivenValueWithLeadingSpaces_WhenCreatingContact_ThenValueShouldBeTrimmed()
    {
        var contact = new Contact(
            ValidOrgId,
            "  info@example.com  ",
            "  Описание  ",
            ContactType.Email
        );

        contact.Value.ShouldBe("info@example.com");
        contact.Description.ShouldBe("Описание");
    }

    [Test]
    public void GivenNoIsPrimaryArgument_WhenCreatingContact_ThenIsPrimaryShouldBeFalseByDefault()
    {
        var contact = new Contact(ValidOrgId, "info@example.com", "Описание", ContactType.Email);

        contact.IsPrimary.ShouldBeFalse();
    }
}
