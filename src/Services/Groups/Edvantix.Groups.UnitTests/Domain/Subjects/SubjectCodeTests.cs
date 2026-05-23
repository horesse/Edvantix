using Edvantix.Groups.Domain.AggregatesModel.SubjectAggregate;

namespace Edvantix.Groups.UnitTests.Domain.Subjects;

public sealed class SubjectCodeTests
{
    [Test]
    public void GivenValidCode_WhenCreating_ThenSucceeds()
    {
        var code = SubjectCode.From("MATH");

        code.Value.ShouldBe("MATH");
    }

    [Test]
    public void GivenLowercaseCode_WhenCreating_ThenNormalizesToUppercase()
    {
        var code = SubjectCode.From("math");

        code.Value.ShouldBe("MATH");
    }

    [Test]
    public void GivenMaxLengthCode_WhenCreating_ThenSucceeds()
    {
        var code = SubjectCode.From("ABCDE12345"); // 10 chars

        code.Value.ShouldBe("ABCDE12345");
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("   ")]
    public void GivenNullOrEmptyCode_WhenCreating_ThenThrowsArgumentException(string? code)
    {
        var act = () => SubjectCode.From(code!);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenTooLongCode_WhenCreating_ThenThrowsArgumentException()
    {
        var code = "ABCDEFGHIJK"; // 11 chars, max is 10

        var act = () => SubjectCode.From(code);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    [Arguments("AB-CD")]
    [Arguments("AB_CD")]
    [Arguments("MATH!")]
    [Arguments("MAT H")]
    public void GivenInvalidFormatCode_WhenCreating_ThenThrowsArgumentException(string code)
    {
        var act = () => SubjectCode.From(code);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenSameValues_WhenComparing_ThenAreEqual()
    {
        var code1 = SubjectCode.From("MATH");
        var code2 = SubjectCode.From("MATH");

        code1.ShouldBe(code2);
    }
}
