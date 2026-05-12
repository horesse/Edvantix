namespace Edvantix.Schedule.UnitTests.Domain;

public sealed class HolidayTests
{
    [Test]
    public void GivenValidData_WhenCreatingHoliday_ThenShouldNormalizeText()
    {
        var date = new DateOnly(2026, 1, 1);

        var holiday = new Holiday(" blr ", date, "  New year  ", isRecurringAnnually: false);

        holiday.CountryCode.ShouldBe("BLR");
        holiday.Date.ShouldBe(date);
        holiday.Name.ShouldBe("New year");
        holiday.IsRecurringAnnually.ShouldBeFalse();
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("   ")]
    public void GivenEmptyCountryCode_WhenCreatingHoliday_ThenShouldThrow(string? countryCode)
    {
        var act = () => new Holiday(countryCode!, new DateOnly(2026, 1, 1), "Holiday");

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("   ")]
    public void GivenEmptyName_WhenCreatingHoliday_ThenShouldThrow(string? name)
    {
        var act = () => new Holiday("BLR", new DateOnly(2026, 1, 1), name!);

        act.ShouldThrow<ArgumentException>();
    }
}
