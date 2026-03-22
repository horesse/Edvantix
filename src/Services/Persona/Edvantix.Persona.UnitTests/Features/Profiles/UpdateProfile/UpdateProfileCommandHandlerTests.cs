using Edvantix.Chassis.Specification;
using Edvantix.Persona.Features.Profiles.Update;
using Edvantix.Persona.UnitTests.Fakers;
using Edvantix.Persona.UnitTests.Helpers;

namespace Edvantix.Persona.UnitTests.Features.Profiles.UpdateProfile;

public sealed class UpdateProfileCommandHandlerTests
{
    private readonly Mock<IProfileRepository> _profileRepoMock = new();
    private readonly Mock<ISkillRepository> _skillRepoMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    public UpdateProfileCommandHandlerTests()
    {
        _profileRepoMock.Setup(r => r.UnitOfWork).Returns(_unitOfWorkMock.Object);
    }

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenShouldUpdateProfileAndReturnProfileId()
    {
        var accountId = Guid.CreateVersion7();
        var profile = CreateProfile(accountId);
        var handler = CreateHandler(accountId);

        SetupProfileFound(profile);
        _unitOfWorkMock
            .Setup(u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await handler.Handle(BuildCommand(), CancellationToken.None);

        result.ShouldBe(accountId);
        _unitOfWorkMock.Verify(u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenShouldUpdateAllScalarFields()
    {
        var accountId = Guid.CreateVersion7();
        var profile = CreateProfile(accountId);
        var handler = CreateHandler(accountId);
        var newBirthDate = new DateOnly(1985, 3, 25);

        SetupProfileFound(profile);
        _unitOfWorkMock
            .Setup(u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var command = new UpdateProfileCommand(
            "Пётр",
            "Петров",
            "Иванович",
            newBirthDate,
            "Описание о себе",
            [],
            [],
            [],
            []
        );

        await handler.Handle(command, CancellationToken.None);

        profile.FullName.FirstName.ShouldBe("Пётр");
        profile.FullName.LastName.ShouldBe("Петров");
        profile.FullName.MiddleName.ShouldBe("Иванович");
        profile.BirthDate.ShouldBe(newBirthDate);
        profile.Bio.ShouldBe("Описание о себе");
    }

    [Test]
    public async Task GivenProfileNotFound_WhenHandling_ThenShouldThrowNotFoundException()
    {
        var handler = CreateHandler(Guid.CreateVersion7());

        _profileRepoMock
            .Setup(r =>
                r.FindAsync(It.IsAny<ISpecification<Profile>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync((Profile?)null);

        await Should.ThrowAsync<NotFoundException>(() =>
            handler.Handle(BuildCommand(), CancellationToken.None).AsTask()
        );

        _unitOfWorkMock.Verify(
            u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Test]
    public async Task GivenExistingSkillName_WhenHandling_ThenShouldReuseExistingSkillId()
    {
        var accountId = Guid.CreateVersion7();
        var profile = CreateProfile(accountId);
        var existingSkill = SkillFaker.Create("C#");
        var handler = CreateHandler(accountId);

        SetupProfileFound(profile);
        _unitOfWorkMock
            .Setup(u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _skillRepoMock
            .Setup(r =>
                r.FindAsync(It.IsAny<ISpecification<Skill>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(existingSkill);

        var command = BuildCommand(skills: ["C#"]);

        await handler.Handle(command, CancellationToken.None);

        _skillRepoMock.Verify(
            r => r.FindAsync(It.IsAny<ISpecification<Skill>>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
        _skillRepoMock.Verify(
            r => r.AddAsync(It.IsAny<Skill>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
        profile.Skills.ShouldHaveSingleItem();
        profile.Skills.First().SkillId.ShouldBe(existingSkill.Id);
    }

    [Test]
    public async Task GivenNewSkillName_WhenHandling_ThenShouldCreateSkillInCatalog()
    {
        var accountId = Guid.CreateVersion7();
        var profile = CreateProfile(accountId);
        var newSkill = SkillFaker.Create("Rust");
        var handler = CreateHandler(accountId);

        SetupProfileFound(profile);
        _unitOfWorkMock
            .Setup(u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _skillRepoMock
            .Setup(r =>
                r.FindAsync(It.IsAny<ISpecification<Skill>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync((Skill?)null);
        _skillRepoMock
            .Setup(r => r.AddAsync(It.IsAny<Skill>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(newSkill);

        var command = BuildCommand(skills: ["Rust"]);

        await handler.Handle(command, CancellationToken.None);

        _skillRepoMock.Verify(
            r => r.AddAsync(It.IsAny<Skill>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
        profile.Skills.ShouldHaveSingleItem();
        profile.Skills.First().SkillId.ShouldBe(newSkill.Id);
    }

    [Test]
    public async Task GivenDuplicateSkillNamesIgnoringCase_WhenHandling_ThenShouldDeduplicateAndAddOnce()
    {
        var accountId = Guid.CreateVersion7();
        var profile = CreateProfile(accountId);
        var existingSkill = SkillFaker.Create("golang");
        var handler = CreateHandler(accountId);

        SetupProfileFound(profile);
        _unitOfWorkMock
            .Setup(u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _skillRepoMock
            .Setup(r =>
                r.FindAsync(It.IsAny<ISpecification<Skill>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(existingSkill);

        // "golang", "GoLang", "GOLANG" — три дубликата
        var command = BuildCommand(skills: ["golang", "GoLang", "GOLANG"]);

        await handler.Handle(command, CancellationToken.None);

        _skillRepoMock.Verify(
            r => r.FindAsync(It.IsAny<ISpecification<Skill>>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
        profile.Skills.ShouldHaveSingleItem();
    }

    private void SetupProfileFound(Profile profile)
    {
        _profileRepoMock
            .Setup(r =>
                r.FindAsync(It.IsAny<ISpecification<Profile>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(profile);
    }

    private UpdateProfileCommandHandler CreateHandler(Guid accountId)
    {
        var claims = ServiceProviderHelper.CreateClaimsPrincipal(accountId);

        return new UpdateProfileCommandHandler(
            claims,
            _profileRepoMock.Object,
            _skillRepoMock.Object
        );
    }

    private static Profile CreateProfile(Guid accountId)
    {
        var profile = new Profile(
            accountId,
            "testuser",
            Gender.Male,
            new DateOnly(1990, 1, 1),
            "Иван",
            "Иванов"
        );
        profile.Id = Guid.CreateVersion7();

        return profile;
    }

    private static UpdateProfileCommand BuildCommand(List<string>? skills = null) =>
        new("Иван", "Иванов", null, new DateOnly(1990, 1, 1), null, [], [], [], skills ?? []);
}
