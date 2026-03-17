using Edvantix.Chassis.Repository;
using Edvantix.Chassis.Specification;

namespace Edvantix.Persona.UnitTests.Domain.EventHandlers;

public sealed class SkillDomainEventHandlerTests
{
    private readonly Mock<ISkillRepository> _skillRepoMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly SkillDomainEventHandler _handler;

    public SkillDomainEventHandlerTests()
    {
        _skillRepoMock.Setup(r => r.UnitOfWork).Returns(_unitOfWorkMock.Object);
        _handler = new SkillDomainEventHandler(_skillRepoMock.Object, Mock.Of<ILogger<SkillDomainEventHandler>>());
    }

    [Test]
    public async Task GivenSkillStillUsedByOtherProfiles_WhenHandling_ThenShouldNotDeleteSkill()
    {
        var skillId = Guid.CreateVersion7();
        var @event = new SkillRemovedFromProfileDomainEvent(Guid.CreateVersion7(), skillId);
        _skillRepoMock
            .Setup(r => r.IsUsedByAnyProfileAsync(skillId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _handler.Handle(@event, CancellationToken.None);

        _skillRepoMock.Verify(
            r => r.FindAsync(It.IsAny<ISpecification<Skill>>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
        _skillRepoMock.Verify(r => r.Remove(It.IsAny<Skill>()), Times.Never);
        _unitOfWorkMock.Verify(
            u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Test]
    public async Task GivenSkillNotFoundInCatalog_WhenHandling_ThenShouldNotCallRemoveOrSave()
    {
        var skillId = Guid.CreateVersion7();
        var @event = new SkillRemovedFromProfileDomainEvent(Guid.CreateVersion7(), skillId);
        _skillRepoMock
            .Setup(r => r.IsUsedByAnyProfileAsync(skillId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _skillRepoMock
            .Setup(r =>
                r.FindAsync(It.IsAny<ISpecification<Skill>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync((Skill?)null);

        await _handler.Handle(@event, CancellationToken.None);

        _skillRepoMock.Verify(r => r.Remove(It.IsAny<Skill>()), Times.Never);
        _unitOfWorkMock.Verify(
            u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Test]
    public async Task GivenUnusedSkillFoundInCatalog_WhenHandling_ThenShouldRemoveSkillAndSave()
    {
        var skillId = Guid.CreateVersion7();
        var @event = new SkillRemovedFromProfileDomainEvent(Guid.CreateVersion7(), skillId);
        var skill = CreateSkill(skillId, "C#");
        _skillRepoMock
            .Setup(r => r.IsUsedByAnyProfileAsync(skillId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _skillRepoMock
            .Setup(r =>
                r.FindAsync(It.IsAny<ISpecification<Skill>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(skill);
        _unitOfWorkMock
            .Setup(u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _handler.Handle(@event, CancellationToken.None);

        _skillRepoMock.Verify(r => r.Remove(skill), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()), Times.Once);
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
