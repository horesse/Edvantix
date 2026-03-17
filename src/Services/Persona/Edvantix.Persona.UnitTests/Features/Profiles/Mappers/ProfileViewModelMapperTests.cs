using Edvantix.Persona.Features.Profiles.Mappers;

namespace Edvantix.Persona.UnitTests.Features.Profiles.Mappers;

public sealed class ProfileViewModelMapperTests
{
    private readonly Mock<IBlobService> _blobServiceMock = new();
    private readonly ProfileViewModelMapper _mapper;

    public ProfileViewModelMapperTests()
    {
        _mapper = new(_blobServiceMock.Object);
    }

    [Test]
    public void GivenProfileWithAvatar_WhenMapping_ThenShouldGenerateSasUrl()
    {
        const string avatarUrn = "urn:blob:avatars/photo.jpg";
        const string sasUrl = "https://storage.example.com/avatars/photo.jpg?sas=token";
        var profile = CreateProfile(avatarUrn: avatarUrn);

        _blobServiceMock.Setup(b => b.GetFileSasUrl(avatarUrn)).Returns(sasUrl);

        var result = _mapper.Map(profile);

        result.AvatarUrl.ShouldBe(sasUrl);
        _blobServiceMock.Verify(b => b.GetFileSasUrl(avatarUrn), Times.Once);
    }

    [Test]
    public void GivenProfileWithoutAvatar_WhenMapping_ThenAvatarUrlShouldBeNull()
    {
        var profile = CreateProfile(avatarUrn: null);

        var result = _mapper.Map(profile);

        result.AvatarUrl.ShouldBeNull();
        _blobServiceMock.Verify(b => b.GetFileSasUrl(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void GivenProfile_WhenMapping_ThenShouldMapIdAndLoginCorrectly()
    {
        var profileId = Guid.CreateVersion7();
        var profile = CreateProfile(profileId: profileId, login: "john.doe");

        var result = _mapper.Map(profile);

        result.Id.ShouldBe(profileId);
        result.UserName.ShouldBe("john.doe");
    }

    [Test]
    public void GivenProfileWithMiddleName_WhenMapping_ThenNameShouldIncludeAllParts()
    {
        var profile = CreateProfile(firstName: "Иван", lastName: "Иванов", middleName: "Петрович");

        var result = _mapper.Map(profile);

        result.Name.ShouldBe("Иванов Иван Петрович");
    }

    [Test]
    public void GivenProfileWithoutMiddleName_WhenMapping_ThenNameShouldNotHaveTrailingSpace()
    {
        var profile = CreateProfile(firstName: "Иван", lastName: "Иванов");

        var result = _mapper.Map(profile);

        result.Name.ShouldBe("Иванов Иван");
    }

    private static Profile CreateProfile(
        Guid? profileId = null,
        string login = "testuser",
        string firstName = "Иван",
        string lastName = "Иванов",
        string? middleName = null,
        string? avatarUrn = null
    )
    {
        var profile = new Profile(
            Guid.CreateVersion7(),
            login,
            Gender.Male,
            new DateOnly(1990, 1, 1),
            firstName,
            lastName,
            middleName
        ) { Id = profileId ?? Guid.CreateVersion7() };

        if (avatarUrn is not null)
            profile.UploadAvatar(avatarUrn);

        return profile;
    }
}
