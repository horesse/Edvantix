namespace Edvantix.Organizational.UnitTests.Domain;

public sealed class FeatureAggregateTests
{
    // ─── Constructor ───────────────────────────────────────────────────────────

    [Test]
    public void GivenValidData_WhenCreatingFeature_ThenShouldInitializePropertiesCorrectly()
    {
        var feature = new Feature("organizational", "Organization", "Организация");

        feature.ServiceCode.ShouldBe("organizational");
        feature.Code.ShouldBe("Organization");
        feature.Name.ShouldBe("Организация");
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("   ")]
    public void GivenNullOrWhiteSpaceServiceCode_WhenCreatingFeature_ThenShouldThrowArgumentException(
        string? serviceCode
    )
    {
        var act = () => new Feature(serviceCode!, "Organization", "Организация");

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("   ")]
    public void GivenNullOrWhiteSpaceCode_WhenCreatingFeature_ThenShouldThrowArgumentException(
        string? code
    )
    {
        var act = () => new Feature("organizational", code!, "Организация");

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("   ")]
    public void GivenNullOrWhiteSpaceName_WhenCreatingFeature_ThenShouldThrowArgumentException(
        string? name
    )
    {
        var act = () => new Feature("organizational", "Organization", name!);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenDataWithLeadingAndTrailingSpaces_WhenCreatingFeature_ThenShouldTrimAllValues()
    {
        var feature = new Feature("  organizational  ", "  Organization  ", "  Организация  ");

        feature.ServiceCode.ShouldBe("organizational");
        feature.Code.ShouldBe("Organization");
        feature.Name.ShouldBe("Организация");
    }

    // ─── Update ───────────────────────────────────────────────────────────────

    [Test]
    public void GivenNewName_WhenUpdating_ThenShouldUpdateName()
    {
        var feature = new Feature("organizational", "Organization", "Организация");

        feature.Update("Новое название");

        feature.Name.ShouldBe("Новое название");
    }

    [Test]
    public void GivenUpdate_WhenUpdating_ThenShouldPreserveOtherProperties()
    {
        var feature = new Feature("organizational", "Organization", "Организация");

        feature.Update("Новое название");

        feature.ServiceCode.ShouldBe("organizational");
        feature.Code.ShouldBe("Organization");
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("   ")]
    public void GivenNullOrWhiteSpaceName_WhenUpdating_ThenShouldThrowArgumentException(
        string? name
    )
    {
        var feature = new Feature("organizational", "Organization", "Организация");

        var act = () => feature.Update(name!);

        act.ShouldThrow<ArgumentException>();
    }

    // ─── Default constructor (EF Core) ────────────────────────────────────────

    [Test]
    public void GivenDefaultConstructor_WhenCreatingFeature_ThenShouldInitializeWithEmptyStrings()
    {
        var feature = Activator.CreateInstance<Feature>();

        feature.ServiceCode.ShouldBe(string.Empty);
        feature.Code.ShouldBe(string.Empty);
        feature.Name.ShouldBe(string.Empty);
    }
}
