using Edvantix.Persona.Features.Profiles.UpdateAvatar;

namespace Edvantix.Persona.UnitTests.Features.Profiles.UpdateAvatar;

public sealed class UpdateAvatarValidatorTests
{
    private readonly UpdateAvatarValidator _validator = new();

    [Test]
    public void GivenValidJpegAvatar_WhenValidating_ThenShouldNotHaveAnyValidationErrors()
    {
        var command = new UpdateAvatarCommand { Avatar = CreateAvatarFile("image/jpeg", 512 * 1024) };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public void GivenValidPngAvatar_WhenValidating_ThenShouldNotHaveAnyValidationErrors()
    {
        var command = new UpdateAvatarCommand { Avatar = CreateAvatarFile("image/png", 256 * 1024) };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public void GivenAvatarExceedingMaxFileSize_WhenValidating_ThenShouldHaveValidationError()
    {
        var command = new UpdateAvatarCommand
        {
            Avatar = CreateAvatarFile("image/jpeg", 2 * 1024 * 1024), // 2 MB
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Avatar.Length);
    }

    [Test]
    [Arguments("image/gif")]
    [Arguments("image/webp")]
    [Arguments("application/octet-stream")]
    public void GivenUnsupportedContentType_WhenValidating_ThenShouldHaveValidationError(string contentType)
    {
        var command = new UpdateAvatarCommand
        {
            Avatar = CreateAvatarFile(contentType, 512 * 1024),
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Avatar.ContentType);
    }

    [Test]
    public void GivenAvatarAtMaxFileSize_WhenValidating_ThenShouldNotHaveValidationError()
    {
        const int maxFileSize = 1048576; // 1 MB
        var command = new UpdateAvatarCommand { Avatar = CreateAvatarFile("image/png", maxFileSize) };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    private static IFormFile CreateAvatarFile(string contentType, long length)
    {
        var mock = new Mock<IFormFile>();
        mock.Setup(f => f.ContentType).Returns(contentType);
        mock.Setup(f => f.Length).Returns(length);

        return mock.Object;
    }
}
