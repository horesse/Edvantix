namespace Edvantix.Groups.UnitTests.Features.Directories.Levels;

public sealed class LevelDirectoryMapperTests
{
    private readonly Guid _orgId = Guid.CreateVersion7();

    [Test]
    public void GivenActiveLevel_WhenMappingToDto_ThenIsArchivedFalse()
    {
        var level = CreateLevel();

        var dto = LevelDirectoryMapper.ToDto(level);

        dto.Id.ShouldBe(level.Id);
        dto.Name.ShouldBe(level.Name);
        dto.Order.ShouldBe(level.SortOrder);
        dto.Description.ShouldBe(level.Description);
        dto.IsArchived.ShouldBeFalse();
        dto.Code.ShouldBe(level.Code.Value);
        dto.Tone.ShouldBe(level.Tone);
    }

    [Test]
    public void GivenDeactivatedLevel_WhenMappingToDto_ThenIsArchivedTrue()
    {
        var level = CreateLevel();
        level.Deactivate();

        var dto = LevelDirectoryMapper.ToDto(level);

        dto.IsArchived.ShouldBeTrue();
    }

    [Test]
    public void GivenActiveLevel_WhenMappingToListItemDto_ThenIsArchivedFalse()
    {
        var level = CreateLevel();

        var dto = LevelDirectoryMapper.ToListItemDto(level);

        dto.Id.ShouldBe(level.Id);
        dto.Name.ShouldBe(level.Name);
        dto.Order.ShouldBe(level.SortOrder);
        dto.IsArchived.ShouldBeFalse();
    }

    [Test]
    public void GivenDeactivatedLevel_WhenMappingToListItemDto_ThenIsArchivedTrue()
    {
        var level = CreateLevel();
        level.Deactivate();

        var dto = LevelDirectoryMapper.ToListItemDto(level);

        dto.IsArchived.ShouldBeTrue();
    }

    [Test]
    public void GivenLevelWithDescription_WhenMapping_ThenDescriptionPreserved()
    {
        var level = CreateLevel(description: "Some description");

        var dto = LevelDirectoryMapper.ToDto(level);

        dto.Description.ShouldBe("Some description");
    }

    private Level CreateLevel(string description = null!) =>
        new(
            _orgId,
            LevelCode.From("A1"),
            "Beginner",
            description,
            LevelTone.Blue,
            sortOrder: 1
        );
}
