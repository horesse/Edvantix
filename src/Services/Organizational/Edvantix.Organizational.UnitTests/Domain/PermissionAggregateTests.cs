namespace Edvantix.Organizational.UnitTests.Domain;

public sealed class PermissionAggregateTests
{
    // ─── Constructor – valid data ──────────────────────────────────────────────

    [Test]
    public void GivenValidData_WhenCreatingPermission_ThenShouldInitializeAllPropertiesCorrectly()
    {
        var permission = new Permission(
            "organizational",
            "Organization",
            "Организация",
            "View",
            "Просмотр"
        );

        permission.ServiceCode.ShouldBe("organizational");
        permission.FeatureCode.ShouldBe("Organization");
        permission.FeatureName.ShouldBe("Организация");
        permission.Code.ShouldBe("View");
        permission.Name.ShouldBe("Просмотр");
    }

    // ─── Constructor – Guard validations ──────────────────────────────────────

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("   ")]
    public void GivenNullOrWhiteSpaceServiceCode_WhenCreatingPermission_ThenShouldThrowArgumentException(
        string? serviceCode
    )
    {
        var act = () =>
            new Permission(serviceCode!, "Organization", "Организация", "View", "Просмотр");

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("   ")]
    public void GivenNullOrWhiteSpaceFeatureCode_WhenCreatingPermission_ThenShouldThrowArgumentException(
        string? featureCode
    )
    {
        var act = () =>
            new Permission("organizational", featureCode!, "Организация", "View", "Просмотр");

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("   ")]
    public void GivenNullOrWhiteSpaceFeatureName_WhenCreatingPermission_ThenShouldThrowArgumentException(
        string? featureName
    )
    {
        var act = () =>
            new Permission("organizational", "Organization", featureName!, "View", "Просмотр");

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("   ")]
    public void GivenNullOrWhiteSpaceCode_WhenCreatingPermission_ThenShouldThrowArgumentException(
        string? code
    )
    {
        var act = () =>
            new Permission("organizational", "Organization", "Организация", code!, "Просмотр");

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("   ")]
    public void GivenNullOrWhiteSpaceName_WhenCreatingPermission_ThenShouldThrowArgumentException(
        string? name
    )
    {
        var act = () =>
            new Permission("organizational", "Organization", "Организация", "View", name!);

        act.ShouldThrow<ArgumentException>();
    }

    // ─── Constructor – trim whitespace ────────────────────────────────────────

    [Test]
    public void GivenDataWithLeadingAndTrailingSpaces_WhenCreatingPermission_ThenShouldTrimAllStringValues()
    {
        var permission = new Permission(
            "  organizational  ",
            "  Organization  ",
            "  Организация  ",
            "  View  ",
            "  Просмотр  "
        );

        permission.ServiceCode.ShouldBe("organizational");
        permission.FeatureCode.ShouldBe("Organization");
        permission.FeatureName.ShouldBe("Организация");
        permission.Code.ShouldBe("View");
        permission.Name.ShouldBe("Просмотр");
    }

    // ─── FullCode ──────────────────────────────────────────────────────────────

    [Test]
    public void GivenValidPermission_WhenAccessingFullCode_ThenShouldReturnFeatureCodeDotCode()
    {
        var permission = CreateValidPermission();

        permission.FullCode.ShouldBe("Organization.View");
    }

    [Test]
    public void GivenDifferentFeatureAndCode_WhenAccessingFullCode_ThenShouldCombineCorrectly()
    {
        var permission = new Permission(
            "organizational",
            "Member",
            "Участник",
            "Delete",
            "Удаление"
        );

        permission.FullCode.ShouldBe("Member.Delete");
    }

    // ─── Update ───────────────────────────────────────────────────────────────

    [Test]
    public void GivenValidData_WhenUpdating_ThenShouldUpdateFeatureNameAndName()
    {
        var permission = CreateValidPermission();

        permission.Update("Новая область", "Новое название");

        permission.FeatureName.ShouldBe("Новая область");
        permission.Name.ShouldBe("Новое название");
    }

    [Test]
    public void GivenUpdateWithSpaces_WhenUpdating_ThenShouldTrimValues()
    {
        var permission = CreateValidPermission();

        permission.Update("  Область  ", "  Название  ");

        permission.FeatureName.ShouldBe("Область");
        permission.Name.ShouldBe("Название");
    }

    [Test]
    public void GivenUpdateDoesNotChangeCodeOrServiceCode_WhenUpdating_ThenShouldPreserveOtherProperties()
    {
        var permission = CreateValidPermission();

        permission.Update("Другая область", "Другое название");

        permission.ServiceCode.ShouldBe("organizational");
        permission.FeatureCode.ShouldBe("Organization");
        permission.Code.ShouldBe("View");
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("   ")]
    public void GivenNullOrWhiteSpaceFeatureName_WhenUpdating_ThenShouldThrowArgumentException(
        string? featureName
    )
    {
        var permission = CreateValidPermission();

        var act = () => permission.Update(featureName!, "Название");

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("   ")]
    public void GivenNullOrWhiteSpaceName_WhenUpdating_ThenShouldThrowArgumentException(
        string? name
    )
    {
        var permission = CreateValidPermission();

        var act = () => permission.Update("Область", name!);

        act.ShouldThrow<ArgumentException>();
    }

    // ─── Default constructor (EF Core) ────────────────────────────────────────

    [Test]
    public void GivenDefaultConstructor_WhenCreatingPermission_ThenShouldInitializeWithEmptyStrings()
    {
        // Parameterless constructor is required by EF Core; access it via Activator since it's public
        var permission = Activator.CreateInstance<Permission>();

        permission.ServiceCode.ShouldBe(string.Empty);
        permission.FeatureCode.ShouldBe(string.Empty);
        permission.FeatureName.ShouldBe(string.Empty);
        permission.Code.ShouldBe(string.Empty);
        permission.Name.ShouldBe(string.Empty);
    }

    // ─── Helpers ──────────────────────────────────────────────────────────────

    private static Permission CreateValidPermission() =>
        new("organizational", "Organization", "Организация", "View", "Просмотр");
}
