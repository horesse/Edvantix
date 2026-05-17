namespace Edvantix.Groups.UnitTests.Domain.Levels;

public sealed class LevelTests
{
    private static readonly Guid ValidOrgId = Guid.CreateVersion7();
    private static readonly LevelCode ValidCode = LevelCode.From("A1");

    private static Level CreateValidLevel() =>
        new(ValidOrgId, ValidCode, "Beginner", null, LevelTone.Blue, sortOrder: 1);

    [Test]
    public void GivenValidParameters_WhenConstructing_ThenLevelIsCreated()
    {
        var level = new Level(
            ValidOrgId,
            ValidCode,
            "Beginner",
            "Начальный уровень",
            LevelTone.Blue,
            sortOrder: 1
        );

        level.OrganizationId.ShouldBe(ValidOrgId);
        level.Code.ShouldBe(ValidCode);
        level.Name.ShouldBe("Beginner");
        level.Description.ShouldBe("Начальный уровень");
        level.Tone.ShouldBe(LevelTone.Blue);
        level.SortOrder.ShouldBe((short)1);
        level.IsActive.ShouldBeTrue();
        level.IsDeleted.ShouldBeFalse();
    }

    [Test]
    public void GivenEmptyOrganizationId_WhenConstructing_ThenThrowsArgumentException()
    {
        var act = () => new Level(Guid.Empty, ValidCode, "Beginner", null, LevelTone.Blue, 1);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("   ")]
    public void GivenNullOrEmptyName_WhenConstructing_ThenThrowsArgumentException(string? name)
    {
        var act = () => new Level(ValidOrgId, ValidCode, name!, null, LevelTone.Blue, 1);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenTooLongName_WhenConstructing_ThenThrowsArgumentException()
    {
        var name = new string('A', 65); // 65 symbols, max is 64

        var act = () => new Level(ValidOrgId, ValidCode, name, null, LevelTone.Blue, 1);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenTooLongDescription_WhenConstructing_ThenThrowsArgumentException()
    {
        var description = new string('X', 257); // 257 symbols, max is 256

        var act = () =>
            new Level(ValidOrgId, ValidCode, "Beginner", description, LevelTone.Blue, 1);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenNameWithWhitespace_WhenConstructing_ThenNameIsTrimmed()
    {
        var level = new Level(ValidOrgId, ValidCode, "  Beginner  ", null, LevelTone.Blue, 1);

        level.Name.ShouldBe("Beginner");
    }

    [Test]
    public void GivenActiveLevel_WhenDeactivating_ThenIsActiveFalse()
    {
        var level = CreateValidLevel();

        level.Deactivate();

        level.IsActive.ShouldBeFalse();
    }

    [Test]
    public void GivenInactiveLevel_WhenActivating_ThenIsActiveTrue()
    {
        var level = CreateValidLevel();
        level.Deactivate();

        level.Activate();

        level.IsActive.ShouldBeTrue();
    }

    [Test]
    public void GivenLevel_WhenDeleting_ThenIsDeletedTrue()
    {
        var level = CreateValidLevel();

        level.Delete();

        level.IsDeleted.ShouldBeTrue();
    }

    [Test]
    public void GivenValidData_WhenUpdating_ThenPropertiesChanged()
    {
        var level = CreateValidLevel();

        level.Update("Advanced", "Продвинутый уровень", LevelTone.Red, sortOrder: 5);

        level.Name.ShouldBe("Advanced");
        level.Description.ShouldBe("Продвинутый уровень");
        level.Tone.ShouldBe(LevelTone.Red);
        level.SortOrder.ShouldBe((short)5);
        level.Code.ShouldBe(ValidCode); // Код не изменился
    }

    [Test]
    public void GivenLevel_WhenSetSortOrder_ThenSortOrderChanged()
    {
        var level = CreateValidLevel();

        level.SetSortOrder(10);

        level.SortOrder.ShouldBe((short)10);
    }
}
