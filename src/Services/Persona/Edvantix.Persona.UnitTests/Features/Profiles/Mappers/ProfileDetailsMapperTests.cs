using Edvantix.Persona.Features.Profiles.Mappers;

namespace Edvantix.Persona.UnitTests.Features.Profiles.Mappers;

public sealed class ProfileDetailsMapperTests
{
    private readonly Mock<IBlobService> _blobServiceMock = new();
    private readonly Mock<IMapper<ProfileContact, ContactModel>> _contactMapperMock = new();
    private readonly Mock<
        IMapper<EmploymentHistory, EmploymentHistoryModel>
    > _employmentMapperMock = new();
    private readonly Mock<IMapper<Education, EducationModel>> _educationMapperMock = new();
    private readonly Mock<IMapper<ProfileSkill, SkillModel>> _skillMapperMock = new();
    private readonly ProfileDetailsMapper _mapper;

    public ProfileDetailsMapperTests()
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
        var profileId = Guid.CreateVersion7();
        var accountId = Guid.CreateVersion7();
        var birthDate = new DateOnly(1990, 6, 15);
        var profile = CreateProfile(
            profileId,
            accountId,
            "john.doe",
            Gender.Female,
            birthDate,
            "Анна",
            "Петрова",
            "Ивановна"
        );

        var result = _mapper.Map(profile);

        result.Id.ShouldBe(profileId);
        result.AccountId.ShouldBe(accountId);
        result.Login.ShouldBe("john.doe");
        result.Gender.ShouldBe(Gender.Female);
        result.BirthDate.ShouldBe(birthDate);
        result.FirstName.ShouldBe("Анна");
        result.LastName.ShouldBe("Петрова");
        result.MiddleName.ShouldBe("Ивановна");
    }

    [Test]
    public void GivenProfileWithAvatar_WhenMapping_ThenShouldGenerateSasUrl()
    {
        const string avatarUrn = "urn:blob:avatars/photo.jpg";
        const string sasUrl = "https://storage.example.com/avatars/photo.jpg?sas=token";
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
        const string bio = "Разработчик с 10 годами опыта.";
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

    private static Profile CreateProfile(
        Guid? profileId = null,
        Guid? accountId = null,
        string login = "testuser",
        Gender gender = Gender.Male,
        DateOnly? birthDate = null,
        string firstName = "Иван",
        string lastName = "Иванов",
        string? middleName = null
    )
    {
        var profile = new Profile(
            accountId ?? Guid.CreateVersion7(),
            login,
            gender,
            birthDate ?? new DateOnly(1990, 1, 1),
            firstName,
            lastName,
            middleName
        );
        profile.Id = profileId ?? Guid.CreateVersion7();

        return profile;
    }
}
