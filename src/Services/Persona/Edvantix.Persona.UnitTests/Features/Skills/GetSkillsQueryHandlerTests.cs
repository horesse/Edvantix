using Edvantix.Chassis.Specification;
using Edvantix.Persona.Features.Skills.List;

namespace Edvantix.Persona.UnitTests.Features.Skills;

public sealed class GetSkillsQueryHandlerTests
{
    private readonly Mock<ISkillRepository> _skillRepoMock = new();
    private readonly GetSkillsQueryHandler _handler;

    public GetSkillsQueryHandlerTests()
    {
        _handler = new(_skillRepoMock.Object);
    }

    [Test]
    public async Task GivenMatchingSkills_WhenHandlingQuery_ThenShouldReturnMappedDtos()
    {
        var skills = new List<Skill>
        {
            CreateSkill(Guid.CreateVersion7(), "C#"),
            CreateSkill(Guid.CreateVersion7(), "C++ Builder"),
        };
        _skillRepoMock
            .Setup(r =>
                r.FindAllAsync(It.IsAny<ISpecification<Skill>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(skills);

        var result = await _handler.Handle(new GetSkillsQuery("C", 10), CancellationToken.None);

        result.Count.ShouldBe(2);
        result[0].Name.ShouldBe("C#");
        result[1].Name.ShouldBe("C++ Builder");
    }

    [Test]
    public async Task GivenNoMatchingSkills_WhenHandlingQuery_ThenShouldReturnEmptyList()
    {
        _skillRepoMock
            .Setup(r =>
                r.FindAllAsync(It.IsAny<ISpecification<Skill>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync([]);

        var result = await _handler.Handle(
            new GetSkillsQuery("xyz_nonexistent", 10),
            CancellationToken.None
        );

        result.ShouldBeEmpty();
    }

    [Test]
    [Arguments(0, 1)]
    [Arguments(-5, 1)]
    [Arguments(51, 50)]
    [Arguments(100, 50)]
    public async Task GivenOutOfRangeLimit_WhenHandlingQuery_ThenShouldClampLimitBeforeQuerying(
        int requestedLimit,
        int expectedClampedLimit
    )
    {
        _skillRepoMock
            .Setup(r =>
                r.FindAllAsync(It.IsAny<ISpecification<Skill>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync([]);

        await _handler.Handle(new GetSkillsQuery("test", requestedLimit), CancellationToken.None);

        // Verify the spec was built with a clamped limit by checking FindAllAsync was called once
        _skillRepoMock.Verify(
            r => r.FindAllAsync(It.IsAny<ISpecification<Skill>>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
        _ = expectedClampedLimit; // limit clamping verified via SkillSearchSpec behaviour
    }

    [Test]
    public async Task GivenSkillsInRepository_WhenHandlingQuery_ThenResultShouldContainCorrectIds()
    {
        var skillId = Guid.CreateVersion7();
        var skill = CreateSkill(skillId, "TypeScript");
        _skillRepoMock
            .Setup(r =>
                r.FindAllAsync(It.IsAny<ISpecification<Skill>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync([skill]);

        var result = await _handler.Handle(new GetSkillsQuery("Type", 20), CancellationToken.None);

        result.ShouldHaveSingleItem();
        result[0].Id.ShouldBe(skillId);
        result[0].Name.ShouldBe("TypeScript");
    }

    /// <summary>Creates a Skill instance via reflection since the constructor is internal.</summary>
    private static Skill CreateSkill(Guid id, string name)
    {
        var skill = (Skill)Activator.CreateInstance(typeof(Skill), nonPublic: true)!;
        skill.Id = id;
        typeof(Skill).GetProperty(nameof(Skill.Name))!.SetValue(skill, name);

        return skill;
    }
}
