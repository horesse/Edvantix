using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Organizational.UnitTests.Domain;

/// <summary>
/// Юнит-тесты на базовый агрегат <see cref="OrganizationScopedLookup"/>.
/// Используется тестовый наследник <see cref="FakeLookup"/>, поскольку класс абстрактный.
/// </summary>
public sealed class OrganizationScopedLookupTests
{
    private static readonly Guid OrgId = Guid.CreateVersion7();
    private static readonly Guid UserId = Guid.CreateVersion7();

    private sealed class FakeLookup : OrganizationScopedLookup
    {
        public FakeLookup() { }

        public FakeLookup(Guid orgId, string name, int order = 0, Guid? createdBy = null)
            : base(orgId, name, order, createdBy) { }
    }

    [Test]
    public void GivenValidData_WhenCreatingLookup_ThenPropertiesAreInitialized()
    {
        var lookup = new FakeLookup(OrgId, "  Уровень A1  ", order: 5, createdBy: UserId);

        lookup.OrganizationId.ShouldBe(OrgId);
        lookup.Name.ShouldBe("Уровень A1");
        lookup.Order.ShouldBe(5);
        lookup.IsArchived.ShouldBeFalse();
        lookup.CreatedBy.ShouldBe(UserId);
        lookup.LastModifiedAt.ShouldBeNull();
        lookup.LastModifiedBy.ShouldBeNull();
        lookup.Id.ShouldNotBe(Guid.Empty);
    }

    [Test]
    public void GivenEmptyOrganizationId_WhenCreatingLookup_ThenShouldThrowArgumentException()
    {
        var act = () => new FakeLookup(Guid.Empty, "Уровень A1");

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("   ")]
    public void GivenNullOrWhiteSpaceName_WhenCreatingLookup_ThenShouldThrowArgumentException(
        string? name
    )
    {
        var act = () => new FakeLookup(OrgId, name!);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenNameLongerThanMax_WhenCreatingLookup_ThenShouldThrowArgumentException()
    {
        var name = new string('A', OrganizationScopedLookup.MaxNameLength + 1);

        var act = () => new FakeLookup(OrgId, name);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenNameAtMaxLength_WhenCreatingLookup_ThenShouldSucceed()
    {
        var name = new string('A', OrganizationScopedLookup.MaxNameLength);

        var lookup = new FakeLookup(OrgId, name);

        lookup.Name.ShouldBe(name);
    }

    [Test]
    public void GivenActiveLookup_WhenArchiving_ThenShouldBecomeArchived()
    {
        var lookup = new FakeLookup(OrgId, "X");

        lookup.Archive(UserId);

        lookup.IsArchived.ShouldBeTrue();
        lookup.LastModifiedAt.ShouldNotBeNull();
        lookup.LastModifiedBy.ShouldBe(UserId);
    }

    [Test]
    public void GivenArchivedLookup_WhenArchivingAgain_ThenShouldBeNoop()
    {
        var lookup = new FakeLookup(OrgId, "X");
        lookup.Archive(UserId);
        var firstTouchAt = lookup.LastModifiedAt;

        lookup.Archive(Guid.CreateVersion7());

        lookup.IsArchived.ShouldBeTrue();
        lookup.LastModifiedAt.ShouldBe(firstTouchAt);
        lookup.LastModifiedBy.ShouldBe(UserId);
    }

    [Test]
    public void GivenArchivedLookup_WhenRestoring_ThenShouldBecomeActive()
    {
        var lookup = new FakeLookup(OrgId, "X");
        lookup.Archive(UserId);

        var restoredBy = Guid.CreateVersion7();
        lookup.Restore(restoredBy);

        lookup.IsArchived.ShouldBeFalse();
        lookup.LastModifiedBy.ShouldBe(restoredBy);
    }

    [Test]
    public void GivenActiveLookup_WhenRestoringAgain_ThenShouldBeNoop()
    {
        var lookup = new FakeLookup(OrgId, "X");

        lookup.Restore(UserId);

        lookup.IsArchived.ShouldBeFalse();
        lookup.LastModifiedAt.ShouldBeNull();
        lookup.LastModifiedBy.ShouldBeNull();
    }

    [Test]
    public void GivenValidName_WhenRenaming_ThenShouldUpdateNameAndAudit()
    {
        var lookup = new FakeLookup(OrgId, "Старое");

        lookup.Rename("  Новое  ", UserId);

        lookup.Name.ShouldBe("Новое");
        lookup.LastModifiedBy.ShouldBe(UserId);
        lookup.LastModifiedAt.ShouldNotBeNull();
    }

    [Test]
    public void GivenSameName_WhenRenaming_ThenShouldBeNoop()
    {
        var lookup = new FakeLookup(OrgId, "Имя");

        lookup.Rename("Имя", UserId);

        lookup.LastModifiedAt.ShouldBeNull();
        lookup.LastModifiedBy.ShouldBeNull();
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("   ")]
    public void GivenEmptyName_WhenRenaming_ThenShouldThrowArgumentException(string? name)
    {
        var lookup = new FakeLookup(OrgId, "Имя");

        var act = () => lookup.Rename(name!, UserId);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenNameLongerThanMax_WhenRenaming_ThenShouldThrowArgumentException()
    {
        var lookup = new FakeLookup(OrgId, "Имя");
        var tooLong = new string('A', OrganizationScopedLookup.MaxNameLength + 1);

        var act = () => lookup.Rename(tooLong, UserId);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenNewOrder_WhenSettingOrder_ThenShouldUpdateAudit()
    {
        var lookup = new FakeLookup(OrgId, "X", order: 0);

        lookup.SetOrder(10, UserId);

        lookup.Order.ShouldBe(10);
        lookup.LastModifiedBy.ShouldBe(UserId);
    }

    [Test]
    public void GivenSameOrder_WhenSettingOrder_ThenShouldBeNoop()
    {
        var lookup = new FakeLookup(OrgId, "X", order: 7);

        lookup.SetOrder(7, UserId);

        lookup.LastModifiedAt.ShouldBeNull();
    }

    [Test]
    public void GivenEmptyByGuid_WhenArchiving_ThenLastModifiedByShouldBeNull()
    {
        var lookup = new FakeLookup(OrgId, "X");

        lookup.Archive(Guid.Empty);

        lookup.IsArchived.ShouldBeTrue();
        lookup.LastModifiedBy.ShouldBeNull();
        lookup.LastModifiedAt.ShouldNotBeNull();
    }
}
