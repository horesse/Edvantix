namespace Edvantix.Organizational.UnitTests.Domain;

public sealed class PermissionAggregateTests
{
    // ─── Constructor ───────────────────────────────────────────────────────────

    [Test]
    public void GivenValidData_WhenCreatingPermission_ThenShouldInitializePropertiesCorrectly()
    {
        var permission = new Permission("Organization", "View", "Просмотр");

        permission.FeatureCode.ShouldBe("Organization");
        permission.Code.ShouldBe("View");
        permission.Name.ShouldBe("Просмотр");
        permission.FullCode.ShouldBe("Organization.View");
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("   ")]
    public void GivenNullOrWhiteSpaceFeatureCode_WhenCreatingPermission_ThenShouldThrowArgumentException(
        string? featureCode
    )
    {
        var act = () => new Permission(featureCode!, "View", "Просмотр");

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
        var act = () => new Permission("Organization", code!, "Просмотр");

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
        var act = () => new Permission("Organization", "View", name!);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenDataWithLeadingAndTrailingSpaces_WhenCreatingPermission_ThenShouldTrimAllValues()
    {
        var permission = new Permission("  Organization  ", "  View  ", "  Просмотр  ");

        permission.FeatureCode.ShouldBe("Organization");
        permission.Code.ShouldBe("View");
        permission.Name.ShouldBe("Просмотр");
    }

    // ─── FullCode ──────────────────────────────────────────────────────────────

    [Test]
    public void GivenDifferentFeatureAndCode_WhenAccessingFullCode_ThenShouldCombineWithDot()
    {
        var permission = new Permission("Member", "Delete", "Удаление");

        permission.FullCode.ShouldBe("Member.Delete");
    }

    // ─── Update ───────────────────────────────────────────────────────────────

    [Test]
    public void GivenNewName_WhenUpdating_ThenShouldUpdateName()
    {
        var permission = CreateValidPermission();

        permission.Update("Новое название");

        permission.Name.ShouldBe("Новое название");
    }

    [Test]
    public void GivenUpdateWithSpaces_WhenUpdating_ThenShouldTrimName()
    {
        var permission = CreateValidPermission();

        permission.Update("  Название  ");

        permission.Name.ShouldBe("Название");
    }

    [Test]
    public void GivenUpdate_WhenUpdating_ThenShouldPreserveOtherProperties()
    {
        var permission = CreateValidPermission();

        permission.Update("Другое название");

        permission.FeatureCode.ShouldBe("Organization");
        permission.Code.ShouldBe("View");
        permission.FullCode.ShouldBe("Organization.View");
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

        var act = () => permission.Update(name!);

        act.ShouldThrow<ArgumentException>();
    }

    // ─── Default constructor (EF Core) ────────────────────────────────────────

    [Test]
    public void GivenDefaultConstructor_WhenCreatingPermission_ThenShouldInitializeWithEmptyStrings()
    {
        var permission = Activator.CreateInstance<Permission>();

        permission.FeatureCode.ShouldBe(string.Empty);
        permission.Code.ShouldBe(string.Empty);
        permission.Name.ShouldBe(string.Empty);
    }

    // ─── Helpers ──────────────────────────────────────────────────────────────

    private static Permission CreateValidPermission() => new("Organization", "View", "Просмотр");
}
