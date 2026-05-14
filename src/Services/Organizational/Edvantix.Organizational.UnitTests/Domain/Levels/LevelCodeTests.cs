namespace Edvantix.Organizational.UnitTests.Domain.Levels;

public sealed class LevelCodeTests
{
    [Test]
    [Arguments("A1")]
    [Arguments("B2")]
    [Arguments("C1_ADV")]
    [Arguments("PRE-SCHOOL")]
    [Arguments("ABCDEFGHIJKLMNOP")]
    public void GivenValidValue_WhenCreating_ThenSucceeds(string value)
    {
        var code = LevelCode.From(value);

        code.Value.ShouldBe(value.Trim().ToUpperInvariant());
    }

    [Test]
    public void GivenLowerCaseValue_WhenCreating_ThenNormalizesToUpperCase()
    {
        var code = LevelCode.From("a1");

        code.Value.ShouldBe("A1");
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("   ")]
    public void GivenNullOrWhiteSpace_WhenCreating_ThenThrowsArgumentException(string? value)
    {
        var act = () => LevelCode.From(value!);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    [Arguments("a1!")]
    [Arguments("hello world")]
    [Arguments("LEVEL@2")]
    public void GivenInvalidFormat_WhenCreating_ThenThrowsArgumentException(string value)
    {
        var act = () => LevelCode.From(value);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenTooLongValue_WhenCreating_ThenThrowsArgumentException()
    {
        var value = new string('A', 17); // 17 symbols, max is 16

        var act = () => LevelCode.From(value);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenValidCode_WhenCallingToString_ThenReturnsValue()
    {
        var code = LevelCode.From("B1");

        code.ToString().ShouldBe("B1");
    }
}
