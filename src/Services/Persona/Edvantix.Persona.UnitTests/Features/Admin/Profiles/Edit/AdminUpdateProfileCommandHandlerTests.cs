using Edvantix.Chassis.Specification;
using Edvantix.Contracts;
using Edvantix.Persona.Features.Admin.Profiles.Edit;
using Edvantix.Persona.Features.Profiles.Update;

namespace Edvantix.Persona.UnitTests.Features.Admin.Profiles.Edit;

public sealed class AdminUpdateProfileCommandHandlerTests
{
    private readonly Mock<IProfileRepository> _profileRepoMock = new();
    private readonly Mock<ISkillRepository> _skillRepoMock = new();
    private readonly Mock<IBus> _busMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly AdminUpdateProfileCommandHandler _handler;

    public AdminUpdateProfileCommandHandlerTests()
    {
        _profileRepoMock.Setup(r => r.UnitOfWork).Returns(_unitOfWorkMock.Object);

        _handler = new(
            _profileRepoMock.Object,
            _skillRepoMock.Object,
            _busMock.Object,
            Mock.Of<ILogger<AdminUpdateProfileCommandHandler>>()
        );
    }

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenShouldUpdateProfileAndPublishNotification()
    {
        var profileId = Guid.CreateVersion7();
        var profile = CreateProfile(profileId);
        var command = BuildCommand(profileId);

        _profileRepoMock
            .Setup(r =>
                r.FindAsync(It.IsAny<ISpecification<Profile>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(profile);
        _unitOfWorkMock
            .Setup(u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _handler.Handle(command, CancellationToken.None);

        profile.FullName.FirstName.ShouldBe("Анна");
        profile.FullName.LastName.ShouldBe("Петрова");
        profile.Bio.ShouldBe("Новое описание");
        _unitOfWorkMock.Verify(u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _busMock.Verify(
            b =>
                b.Publish(
                    It.Is<SendInAppNotificationIntegrationEvent>(e => e.ProfileId == profile.Id),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
    }

    [Test]
    public async Task GivenProfileNotFound_WhenHandling_ThenShouldThrowNotFoundException()
    {
        var command = BuildCommand(Guid.CreateVersion7());

        _profileRepoMock
            .Setup(r =>
                r.FindAsync(It.IsAny<ISpecification<Profile>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync((Profile?)null);

        await Should.ThrowAsync<NotFoundException>(() =>
            _handler.Handle(command, CancellationToken.None).AsTask()
        );
    }

    [Test]
    public async Task GivenCommandWithSkills_WhenHandling_ThenShouldResolveSkillIds()
    {
        var profileId = Guid.CreateVersion7();
        var profile = CreateProfile(profileId);
        var skillId = Guid.CreateVersion7();
        var skill = CreateSkill(skillId, "C#");
        var command = BuildCommand(profileId, skills: ["C#"]);

        _profileRepoMock
            .Setup(r =>
                r.FindAsync(It.IsAny<ISpecification<Profile>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(profile);
        _skillRepoMock
            .Setup(r =>
                r.FindAsync(It.IsAny<ISpecification<Skill>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(skill);
        _unitOfWorkMock
            .Setup(u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _handler.Handle(command, CancellationToken.None);

        profile.Skills.Count.ShouldBe(1);
    }

    [Test]
    public async Task GivenNewSkillName_WhenHandling_ThenShouldCreateSkillInRepository()
    {
        var profileId = Guid.CreateVersion7();
        var profile = CreateProfile(profileId);
        var command = BuildCommand(profileId, skills: ["НовыйНавык"]);

        _profileRepoMock
            .Setup(r =>
                r.FindAsync(It.IsAny<ISpecification<Profile>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(profile);
        _skillRepoMock
            .Setup(r =>
                r.FindAsync(It.IsAny<ISpecification<Skill>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync((Skill?)null);
        _skillRepoMock
            .Setup(r => r.AddAsync(It.IsAny<Skill>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                (Skill s, CancellationToken _) =>
                {
                    s.Id = Guid.CreateVersion7();
                    return s;
                }
            );
        _unitOfWorkMock
            .Setup(u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _handler.Handle(command, CancellationToken.None);

        _skillRepoMock.Verify(
            r => r.AddAsync(It.IsAny<Skill>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    private static Profile CreateProfile(Guid profileId) =>
        new(
            Guid.CreateVersion7(),
            "testuser",
            Gender.Male,
            new DateOnly(1990, 1, 1),
            "Иван",
            "Иванов"
        )
        {
            Id = profileId,
        };

    private static Skill CreateSkill(Guid id, string name)
    {
        var skill = (Skill)Activator.CreateInstance(typeof(Skill), nonPublic: true)!;
        skill.Id = id;
        typeof(Skill).GetProperty(nameof(Skill.Name))!.SetValue(skill, name);
        return skill;
    }

    private static AdminUpdateProfileCommand BuildCommand(
        Guid profileId,
        List<string>? skills = null
    ) =>
        new(
            profileId,
            "Анна",
            "Петрова",
            null,
            new DateOnly(1995, 3, 20),
            "Новое описание",
            [],
            [],
            [],
            skills ?? [],
            "Причина обновления"
        );
}
