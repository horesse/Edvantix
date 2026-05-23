using Edvantix.Groups.Features.Directories.Subjects.Create;

namespace Edvantix.Groups.UnitTests.Features.Directories.Subjects.Create;

public sealed class CreateSubjectValidatorTests
{
    private readonly CreateSubjectValidator _validator = new();

    [Test]
    public async Task GivenValidCommand_WhenValidating_ThenShouldPass()
    {
        var command = new CreateSubjectCommand("Математика", "MATH", "#6366F1", null, 0);

        var result = await _validator.ValidateAsync(command);

        result.IsValid.ShouldBeTrue();
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("   ")]
    public async Task GivenEmptyName_WhenValidating_ThenShouldFail(string? name)
    {
        var command = new CreateSubjectCommand(name!, "MATH", "#6366F1");

        var result = await _validator.ValidateAsync(command);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(CreateSubjectCommand.Name));
    }

    [Test]
    public async Task GivenTooLongName_WhenValidating_ThenShouldFail()
    {
        var name = new string('A', 121);
        var command = new CreateSubjectCommand(name, "MATH", "#6366F1");

        var result = await _validator.ValidateAsync(command);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(CreateSubjectCommand.Name));
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("   ")]
    public async Task GivenEmptyCode_WhenValidating_ThenShouldFail(string? code)
    {
        var command = new CreateSubjectCommand("Математика", code!, "#6366F1");

        var result = await _validator.ValidateAsync(command);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(CreateSubjectCommand.Code));
    }

    [Test]
    [Arguments("math")]
    [Arguments("AB-CD")]
    [Arguments("ABCDEFGHIJK")] // 11 chars
    public async Task GivenInvalidCode_WhenValidating_ThenShouldFail(string code)
    {
        var command = new CreateSubjectCommand("Математика", code, "#6366F1");

        var result = await _validator.ValidateAsync(command);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(CreateSubjectCommand.Code));
    }

    [Test]
    [Arguments("")]
    [Arguments("6366F1")]
    [Arguments("#GGGGGG")]
    [Arguments("#6366F")]
    public async Task GivenInvalidColor_WhenValidating_ThenShouldFail(string color)
    {
        var command = new CreateSubjectCommand("Математика", "MATH", color);

        var result = await _validator.ValidateAsync(command);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(CreateSubjectCommand.Color));
    }

    [Test]
    public async Task GivenTooLongDescription_WhenValidating_ThenShouldFail()
    {
        var description = new string('X', 501);
        var command = new CreateSubjectCommand("Математика", "MATH", "#6366F1", description);

        var result = await _validator.ValidateAsync(command);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e =>
            e.PropertyName == nameof(CreateSubjectCommand.Description)
        );
    }

    [Test]
    public async Task GivenNegativeOrder_WhenValidating_ThenShouldFail()
    {
        var command = new CreateSubjectCommand("Математика", "MATH", "#6366F1", null, -1);

        var result = await _validator.ValidateAsync(command);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(CreateSubjectCommand.Order));
    }

    [Test]
    public async Task GivenMaxLengthDescription_WhenValidating_ThenShouldPass()
    {
        var description = new string('X', 500);
        var command = new CreateSubjectCommand("Математика", "MATH", "#6366F1", description);

        var result = await _validator.ValidateAsync(command);

        result.IsValid.ShouldBeTrue();
    }
}
