namespace Edvantix.Organizations.UnitTests.Domain;

public sealed class PermissionAggregateTests
{
    [Test]
    public void GivenValidName_WhenCreatingPermission_ThenNameIsSet()
    {
        var permission = new Permission("scheduling:read");

        permission.Name.ShouldBe("scheduling:read");
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("   ")]
    public void GivenEmptyName_WhenCreatingPermission_ThenThrowsArgumentException(string? name)
    {
        var act = () => new Permission(name!);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenPermissionWithWhitespaceName_WhenCreating_ThenNameIsTrimmed()
    {
        var permission = new Permission("  scheduling:write  ");

        permission.Name.ShouldBe("scheduling:write");
    }
}
