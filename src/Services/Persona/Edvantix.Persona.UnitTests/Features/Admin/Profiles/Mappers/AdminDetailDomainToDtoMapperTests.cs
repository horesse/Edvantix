using Edvantix.Persona.Features.Admin.Profiles;
using Edvantix.Persona.Features.Contacts;
using Edvantix.Persona.Features.Educations;
using Edvantix.Persona.Features.EmploymentHistories;

namespace Edvantix.Persona.UnitTests.Features.Admin.Profiles.Mappers;

public sealed class AdminDetailDomainToDtoMapperTests
{
    private readonly Mock<IBlobService> _blobServiceMock = new();
    private readonly Mock<IMapper<ProfileContact, ContactDto>> _contactMapperMock = new();
    private readonly Mock<IMapper<EmploymentHistory, EmploymentHistoryDto>> _employmentMapperMock =
        new();
    private readonly Mock<IMapper<Education, EducationDto>> _educationMapperMock = new();
    private readonly Mock<IMapper<ProfileSkill, SkillDto>> _skillMapperMock = new();
    private readonly AdminDetailDomainToDtoMapper _mapper;

    public AdminDetailDomainToDtoMapperTests()
    {
        _mapper = new(
            _blobServiceMock.Object,
            _contactMapperMock.Object,
            _employmentMapperMock.Object,
            _educationMapperMock.Object,
            _skillMapperMock.Object
        );
    }

    [Test]
    public void GivenProfile_WhenMapping_ThenShouldMapAllScalarFieldsCorrectly()
    {
        var profile = CreateProfile();

        var result = _mapper.Map(profile);

        result.Id.ShouldBe(profile.Id);
        result.AccountId.ShouldBe(profile.AccountId);
        result.UserName.ShouldBe("testuser");
        result.FirstName.ShouldBe("Анна");
        result.LastName.ShouldBe("Петрова");
        result.MiddleName.ShouldBe("Ивановна");
        result.Gender.ShouldBe(Gender.Female);
        result.BirthDate.ShouldBe(new DateOnly(1990, 6, 15));
        result.IsBlocked.ShouldBeFalse();
        result.LastLoginAt.ShouldBeNull();
    }

    [Test]
    public void GivenProfileWithAvatar_WhenMapping_ThenShouldGenerateSasUrl()
    {
        const string avatarUrn = "urn:blob:avatars/photo.jpg";
        const string sasUrl = "https://storage.example.com/photo.jpg?sas=token";
        var profile = CreateProfile();
        profile.UploadAvatar(avatarUrn);

        _blobServiceMock.Setup(b => b.GetFileSasUrl(avatarUrn)).Returns(sasUrl);

        var result = _mapper.Map(profile);

        result.AvatarUrl.ShouldBe(sasUrl);
    }

    [Test]
    public void GivenProfileWithoutAvatar_WhenMapping_ThenAvatarUrlShouldBeNull()
    {
        var profile = CreateProfile();

        var result = _mapper.Map(profile);

        result.AvatarUrl.ShouldBeNull();
        _blobServiceMock.Verify(b => b.GetFileSasUrl(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void GivenProfileWithBio_WhenMapping_ThenBioShouldBePreserved()
    {
        const string bio = "Тестовое описание.";
        var profile = CreateProfile();
        profile.UpdateBio(bio);

        var result = _mapper.Map(profile);

        result.Bio.ShouldBe(bio);
    }

    [Test]
    public void GivenProfileWithEmptyCollections_WhenMapping_ThenAllCollectionsShouldBeEmpty()
    {
        var profile = CreateProfile();

        var result = _mapper.Map(profile);

        result.Contacts.ShouldBeEmpty();
        result.EmploymentHistories.ShouldBeEmpty();
        result.Educations.ShouldBeEmpty();
        result.Skills.ShouldBeEmpty();
    }

    private static Profile CreateProfile() =>
        new(
            Guid.CreateVersion7(),
            "testuser",
            Gender.Female,
            new DateOnly(1990, 6, 15),
            "Анна",
            "Петрова",
            "Ивановна"
        )
        {
            Id = Guid.CreateVersion7(),
        };
}
