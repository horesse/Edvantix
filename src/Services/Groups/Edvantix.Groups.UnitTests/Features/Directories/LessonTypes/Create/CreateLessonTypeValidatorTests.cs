using Edvantix.Groups.Features.Directories.LessonTypes;
using Edvantix.Groups.Features.Directories.LessonTypes.Create;

namespace Edvantix.Groups.UnitTests.Features.Directories.LessonTypes.Create;

public sealed class CreateLessonTypeValidatorTests
{
    private readonly Mock<ILessonTypeUniqueChecker> _uniqueCheckerMock = new();
    private readonly CreateLessonTypeValidator _validator;
    private readonly Guid _orgId = Guid.CreateVersion7();

    public CreateLessonTypeValidatorTests()
    {
        _uniqueCheckerMock
            .Setup(c =>
                c.NameExistsAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<string>(),
                    It.IsAny<Guid?>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(false);

        _uniqueCheckerMock
            .Setup(c =>
                c.CodeExistsAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<string>(),
                    It.IsAny<Guid?>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(false);

        _validator = new(_uniqueCheckerMock.Object);
    }

    private CreateLessonTypeCommand BuildValidCommand() =>
        new(_orgId, "Урок", "LESSON", 45, "#3B82F6", "CalendarDays");

    [Test]
    public async Task GivenValidCommand_WhenValidating_ThenShouldPass()
    {
        var result = await _validator.ValidateAsync(BuildValidCommand());

        result.IsValid.ShouldBeTrue();
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("   ")]
    public async Task GivenEmptyName_WhenValidating_ThenShouldFail(string? name)
    {
        var command = BuildValidCommand() with { Name = name! };

        var result = await _validator.ValidateAsync(command);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(CreateLessonTypeCommand.Name));
    }

    [Test]
    public async Task GivenTooLongName_WhenValidating_ThenShouldFail()
    {
        var command = BuildValidCommand() with { Name = new string('А', 121) };

        var result = await _validator.ValidateAsync(command);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(CreateLessonTypeCommand.Name));
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    public async Task GivenEmptyCode_WhenValidating_ThenShouldFail(string? code)
    {
        var command = BuildValidCommand() with { Code = code! };

        var result = await _validator.ValidateAsync(command);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(CreateLessonTypeCommand.Code));
    }

    [Test]
    [Arguments("lowercase")]
    [Arguments("LESSON TYPE")]
    [Arguments("ABCDEFGHIJKLMNOPQRSTU")] // 21 chars
    public async Task GivenInvalidCode_WhenValidating_ThenShouldFail(string code)
    {
        var command = BuildValidCommand() with { Code = code };

        var result = await _validator.ValidateAsync(command);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(CreateLessonTypeCommand.Code));
    }

    [Test]
    [Arguments(4)]
    [Arguments(601)]
    public async Task GivenOutOfRangeDuration_WhenValidating_ThenShouldFail(int duration)
    {
        var command = BuildValidCommand() with { DefaultDurationMinutes = duration };

        var result = await _validator.ValidateAsync(command);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e =>
            e.PropertyName == nameof(CreateLessonTypeCommand.DefaultDurationMinutes)
        );
    }

    [Test]
    [Arguments("3B82F6")]
    [Arguments("#3B82F")]
    [Arguments("#ZZZZZZ")]
    [Arguments("")]
    public async Task GivenInvalidColor_WhenValidating_ThenShouldFail(string color)
    {
        var command = BuildValidCommand() with { Color = color };

        var result = await _validator.ValidateAsync(command);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(CreateLessonTypeCommand.Color));
    }

    [Test]
    public async Task GivenTooLongIcon_WhenValidating_ThenShouldFail()
    {
        var command = BuildValidCommand() with { Icon = new string('X', 41) };

        var result = await _validator.ValidateAsync(command);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(CreateLessonTypeCommand.Icon));
    }

    [Test]
    public async Task GivenDuplicateName_WhenValidating_ThenShouldFail()
    {
        _uniqueCheckerMock
            .Setup(c => c.NameExistsAsync(_orgId, "Урок", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _validator.ValidateAsync(BuildValidCommand());

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(CreateLessonTypeCommand.Name));
    }

    [Test]
    public async Task GivenDuplicateCode_WhenValidating_ThenShouldFail()
    {
        _uniqueCheckerMock
            .Setup(c => c.CodeExistsAsync(_orgId, "LESSON", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _validator.ValidateAsync(BuildValidCommand());

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(CreateLessonTypeCommand.Code));
    }
}
