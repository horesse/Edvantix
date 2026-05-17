namespace Edvantix.Groups.UnitTests.Features.Levels;

public sealed class LevelDomainToDtoMapperTests
{
    private static readonly Guid ValidOrgId = Guid.CreateVersion7();
    private readonly LevelDomainToDtoMapper _mapper = new();

    [Test]
    public void GivenActiveLevel_WhenMapping_ThenAllFieldsAreMapped()
    {
        var level = new Level(
            ValidOrgId,
            LevelCode.From("B1"),
            "Intermediate",
            "Средний",
            LevelTone.Teal,
            sortOrder: 2
        );

        var dto = _mapper.Map(level);

        dto.Id.ShouldBe(level.Id);
        dto.Code.ShouldBe("B1");
        dto.Name.ShouldBe("Intermediate");
        dto.Description.ShouldBe("Средний");
        dto.Tone.ShouldBe(LevelTone.Teal);
        dto.SortOrder.ShouldBe((short)2);
        dto.IsActive.ShouldBeTrue();
        dto.UsageCount.ShouldBe(0);
    }

    [Test]
    public void GivenLevelWithNullDescription_WhenMapping_ThenDescriptionIsNull()
    {
        var level = new Level(
            ValidOrgId,
            LevelCode.From("A1"),
            "Beginner",
            null,
            LevelTone.Blue,
            sortOrder: 1
        );

        var dto = _mapper.Map(level);

        dto.Description.ShouldBeNull();
    }

    [Test]
    public void GivenDeactivatedLevel_WhenMapping_ThenIsActiveFalse()
    {
        var level = new Level(
            ValidOrgId,
            LevelCode.From("A1"),
            "Beginner",
            null,
            LevelTone.Blue,
            sortOrder: 1
        );
        level.Deactivate();

        var dto = _mapper.Map(level);

        dto.IsActive.ShouldBeFalse();
    }

    [Test]
    public void GivenLevel_WhenMapping_ThenUsageCountIsZeroPlaceholder()
    {
        // UsageCount is a placeholder (0) until Group.LevelId FK is implemented
        var level = new Level(
            ValidOrgId,
            LevelCode.From("C1"),
            "Advanced",
            null,
            LevelTone.Violet,
            sortOrder: 3
        );

        var dto = _mapper.Map(level);

        dto.UsageCount.ShouldBe(0);
    }

    [Test]
    public void GivenLevelWithLowerCaseCode_WhenMapping_ThenCodeIsNormalisedToUpperCase()
    {
        // LevelCode.From normalises to upper-case before storing
        var level = new Level(
            ValidOrgId,
            LevelCode.From("a1"),
            "Beginner",
            null,
            LevelTone.Blue,
            sortOrder: 1
        );

        var dto = _mapper.Map(level);

        dto.Code.ShouldBe("A1");
    }
}
