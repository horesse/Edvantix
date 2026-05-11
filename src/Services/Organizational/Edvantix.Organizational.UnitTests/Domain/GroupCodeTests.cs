namespace Edvantix.Organizational.UnitTests.Domain;

public sealed class GroupCodeTests
{
    [Test]
    [Arguments("EN-B1-12")]
    [Arguments("MATH-A1-01")]
    [Arguments("ABC123")]
    [Arguments("A1")]
    public void GivenValidFormat_WhenCreatingGroupCode_ThenShouldSucceed(string value)
    {
        var code = GroupCode.From(value);

        code.Value.ShouldBe(value.ToUpperInvariant());
    }

    [Test]
    public void GivenLowercaseInput_WhenCreatingGroupCode_ThenShouldNormalizeToUppercase()
    {
        var code = GroupCode.From("en-b1-12");

        code.Value.ShouldBe("EN-B1-12");
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("   ")]
    public void GivenNullOrEmpty_WhenCreatingGroupCode_ThenShouldThrowArgumentException(
        string? value
    )
    {
        var act = () => GroupCode.From(value!);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenValueLongerThan32Chars_WhenCreatingGroupCode_ThenShouldThrowArgumentException()
    {
        var longValue = new string('A', 33);

        var act = () => GroupCode.From(longValue);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    [Arguments("en b1")] // пробел
    [Arguments("EN_B1")] // нижнее подчёркивание
    [Arguments("ЕН-B1")] // кириллица
    [Arguments("EN.B1")] // точка
    public void GivenInvalidCharacters_WhenCreatingGroupCode_ThenShouldThrowArgumentException(
        string value
    )
    {
        var act = () => GroupCode.From(value);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenSameCodes_WhenComparingGroupCodes_ThenShouldBeEqual()
    {
        var code1 = GroupCode.From("EN-B1-12");
        var code2 = GroupCode.From("EN-B1-12");

        code1.ShouldBe(code2);
    }

    [Test]
    public void GivenDifferentCodes_WhenComparingGroupCodes_ThenShouldNotBeEqual()
    {
        var code1 = GroupCode.From("EN-B1-12");
        var code2 = GroupCode.From("EN-B2-01");

        code1.ShouldNotBe(code2);
    }

    [Test]
    public void GivenGroupCode_WhenCallingToString_ThenShouldReturnValue()
    {
        var code = GroupCode.From("EN-B1-12");

        code.ToString().ShouldBe("EN-B1-12");
    }

    [Test]
    public void GivenExactly32Chars_WhenCreatingGroupCode_ThenShouldSucceed()
    {
        var value = new string('A', 32);

        var code = GroupCode.From(value);

        code.Value.ShouldBe(value);
    }
}
