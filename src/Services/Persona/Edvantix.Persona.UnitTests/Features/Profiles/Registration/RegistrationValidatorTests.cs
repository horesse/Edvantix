using Edvantix.Persona.Features.Profiles.Create;

namespace Edvantix.Persona.UnitTests.Features.Profiles.Registration;

public sealed class RegistrationValidatorTests
{
    private readonly Validator _validator = new();

    [Test]
    public void GivenValidCommand_WhenValidating_ThenShouldNotHaveAnyValidationErrors()
    {
        var command = new RegistrationCommand
        {
            FirstName = "Иван",
            LastName = "Иванов",
            MiddleName = "Петрович",
            BirthDate = new DateOnly(1990, 1, 1),
            Gender = Gender.Male,
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("  ")]
    public void GivenEmptyOrNullFirstName_WhenValidating_ThenShouldHaveValidationError(
        string? firstName
    )
    {
        var command = new RegistrationCommand
        {
            FirstName = firstName!,
            LastName = "Иванов",
            BirthDate = new DateOnly(1990, 1, 1),
            Gender = Gender.Male,
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Test]
    public void GivenFirstNameExceedingMaxLength_WhenValidating_ThenShouldHaveValidationError()
    {
        var command = new RegistrationCommand
        {
            FirstName = new string('А', DataSchemaLength.Large + 1),
            LastName = "Иванов",
            BirthDate = new DateOnly(1990, 1, 1),
            Gender = Gender.Male,
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("  ")]
    public void GivenEmptyOrNullLastName_WhenValidating_ThenShouldHaveValidationError(
        string? lastName
    )
    {
        var command = new RegistrationCommand
        {
            FirstName = "Иван",
            LastName = lastName!,
            BirthDate = new DateOnly(1990, 1, 1),
            Gender = Gender.Male,
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.LastName);
    }

    [Test]
    public void GivenLastNameExceedingMaxLength_WhenValidating_ThenShouldHaveValidationError()
    {
        var command = new RegistrationCommand
        {
            FirstName = "Иван",
            LastName = new string('А', DataSchemaLength.Large + 1),
            BirthDate = new DateOnly(1990, 1, 1),
            Gender = Gender.Male,
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.LastName);
    }

    [Test]
    public void GivenMiddleNameExceedingMaxLength_WhenValidating_ThenShouldHaveValidationError()
    {
        var command = new RegistrationCommand
        {
            FirstName = "Иван",
            LastName = "Иванов",
            MiddleName = new string('А', DataSchemaLength.Large + 1),
            BirthDate = new DateOnly(1990, 1, 1),
            Gender = Gender.Male,
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.MiddleName);
    }

    [Test]
    public void GivenInvalidGender_WhenValidating_ThenShouldHaveValidationError()
    {
        var command = new RegistrationCommand
        {
            FirstName = "Иван",
            LastName = "Иванов",
            BirthDate = new DateOnly(1990, 1, 1),
            Gender = (Gender)99,
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Gender);
    }

    [Test]
    public void GivenAvatarExceedingMaxFileSize_WhenValidating_ThenShouldHaveAvatarValidationError()
    {
        var avatarMock = new Mock<IFormFile>();
        avatarMock.Setup(f => f.Length).Returns(2 * 1024 * 1024); // 2 MB
        avatarMock.Setup(f => f.ContentType).Returns("image/jpeg");

        var command = new RegistrationCommand
        {
            FirstName = "Иван",
            LastName = "Иванов",
            BirthDate = new DateOnly(1990, 1, 1),
            Gender = Gender.Male,
            Avatar = avatarMock.Object,
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Avatar!.Length);
    }

    [Test]
    public void GivenAvatarWithInvalidContentType_WhenValidating_ThenShouldHaveAvatarValidationError()
    {
        var avatarMock = new Mock<IFormFile>();
        avatarMock.Setup(f => f.Length).Returns(512 * 1024); // 512 KB
        avatarMock.Setup(f => f.ContentType).Returns("image/gif");

        var command = new RegistrationCommand
        {
            FirstName = "Иван",
            LastName = "Иванов",
            BirthDate = new DateOnly(1990, 1, 1),
            Gender = Gender.Male,
            Avatar = avatarMock.Object,
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Avatar!.ContentType);
    }

    [Test]
    public void GivenNullAvatar_WhenValidating_ThenShouldNotHaveAvatarValidationError()
    {
        var command = new RegistrationCommand
        {
            FirstName = "Иван",
            LastName = "Иванов",
            BirthDate = new DateOnly(1990, 1, 1),
            Gender = Gender.Male,
            Avatar = null,
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Avatar);
    }
}
