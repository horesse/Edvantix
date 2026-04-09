using Edvantix.Persona.Features.Admin.Profiles;

namespace Edvantix.Persona.UnitTests.Features.Admin.Profiles.Mappers;

public sealed class AdminDomainToDtoMapperTests
{
    private readonly Mock<IBlobService> _blobServiceMock = new();
    private readonly AdminDomainToDtoMapper _mapper;

    public AdminDomainToDtoMapperTests()
    {
        _mapper = new(_blobServiceMock.Object);
    }

    [Test]
    public void GivenProfile_WhenMapping_ThenShouldMapAllFieldsCorrectly()
    {
        var profile = CreateProfile();

        var result = _mapper.Map(profile);

        result.Id.ShouldBe(profile.Id);
        result.AccountId.ShouldBe(profile.AccountId);
        result.FullName.ShouldBe("Иванов Иван");
        result.UserName.ShouldBe("testuser");
        result.IsBlocked.ShouldBeFalse();
        result.LastLoginAt.ShouldBeNull();
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
    public void GivenBlockedProfile_WhenMapping_ThenIsBlockedShouldBeTrue()
    {
        var profile = CreateProfile();
        profile.Block();

        var result = _mapper.Map(profile);

        result.IsBlocked.ShouldBeTrue();
    }

    [Test]
    public void GivenProfileWithLastLogin_WhenMapping_ThenLastLoginAtShouldBeMapped()
    {
        var profile = CreateProfile();
        profile.RecordLastLogin();

        var result = _mapper.Map(profile);

        result.LastLoginAt.ShouldNotBeNull();
    }

    private static Profile CreateProfile() =>
        new(
            Guid.CreateVersion7(),
            "testuser",
            Gender.Male,
            new DateOnly(1990, 1, 1),
            "Иван",
            "Иванов"
        )
        {
            Id = Guid.CreateVersion7(),
        };
}
