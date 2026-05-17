namespace Edvantix.Groups.UnitTests.Features.Levels.Create;

public sealed class CreateLevelValidatorTests
{
    private readonly CreateLevelValidator _validator = new();

    [Test]
    public async Task GivenValidCommand_WhenValidating_ThenShouldPass()
    {
        var command = new CreateLevelCommand("A1", "Beginner", null, LevelTone.Blue, SortOrder: 1);

        var result = await _validator.ValidateAsync(command);

        result.IsValid.ShouldBeTrue();
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("   ")]
    public async Task GivenEmptyCode_WhenValidating_ThenShouldFail(string? code)
    {
        var command = new CreateLevelCommand(code!, "Beginner", null, LevelTone.Blue, SortOrder: 1);

        var result = await _validator.ValidateAsync(command);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(CreateLevelCommand.Code));
    }

    [Test]
    [Arguments("a1")]
    [Arguments("hello world")]
    [Arguments("ABCDEFGHIJKLMNOPQ")] // 17 chars
    public async Task GivenInvalidCode_WhenValidating_ThenShouldFail(string code)
    {
        var command = new CreateLevelCommand(code, "Beginner", null, LevelTone.Blue, SortOrder: 1);

        var result = await _validator.ValidateAsync(command);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(CreateLevelCommand.Code));
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("   ")]
    public async Task GivenEmptyName_WhenValidating_ThenShouldFail(string? name)
    {
        var command = new CreateLevelCommand("A1", name!, null, LevelTone.Blue, SortOrder: 1);

        var result = await _validator.ValidateAsync(command);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(CreateLevelCommand.Name));
    }

    [Test]
    public async Task GivenTooLongName_WhenValidating_ThenShouldFail()
    {
        var name = new string('A', 65);
        var command = new CreateLevelCommand("A1", name, null, LevelTone.Blue, SortOrder: 1);

        var result = await _validator.ValidateAsync(command);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(CreateLevelCommand.Name));
    }

    [Test]
    public async Task GivenTooLongDescription_WhenValidating_ThenShouldFail()
    {
        var description = new string('X', 257);
        var command = new CreateLevelCommand(
            "A1",
            "Beginner",
            description,
            LevelTone.Blue,
            SortOrder: 1
        );

        var result = await _validator.ValidateAsync(command);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(CreateLevelCommand.Description));
    }

    [Test]
    public async Task GivenMaxLengthDescription_WhenValidating_ThenShouldPass()
    {
        var description = new string('X', 256);
        var command = new CreateLevelCommand(
            "A1",
            "Beginner",
            description,
            LevelTone.Blue,
            SortOrder: 1
        );

        var result = await _validator.ValidateAsync(command);

        result.IsValid.ShouldBeTrue();
    }
}
