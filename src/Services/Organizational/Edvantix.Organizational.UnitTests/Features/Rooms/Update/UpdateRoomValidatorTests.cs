namespace Edvantix.Organizational.UnitTests.Features.Rooms.Update;

public sealed class UpdateRoomValidatorTests
{
    private readonly UpdateRoomValidator _validator = new();

    [Test]
    public void GivenValidCommand_WhenValidating_ThenShouldNotHaveAnyErrors()
    {
        var result = _validator.TestValidate(
            new UpdateRoomCommand(Guid.CreateVersion7(), "Каб. 204", Floor: 2, Seats: 20)
        );

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public void GivenEmptyId_WhenValidating_ThenShouldHaveError()
    {
        var result = _validator.TestValidate(
            new UpdateRoomCommand(Guid.Empty, "Каб. 204", Floor: 1, Seats: 10)
        );

        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("   ")]
    public void GivenEmptyLabel_WhenValidating_ThenShouldHaveError(string? label)
    {
        var result = _validator.TestValidate(
            new UpdateRoomCommand(Guid.CreateVersion7(), label!, Floor: 1, Seats: 10)
        );

        result.ShouldHaveValidationErrorFor(x => x.Label);
    }

    [Test]
    public void GivenLabelExceeding64Chars_WhenValidating_ThenShouldHaveError()
    {
        var label = new string('А', 65);

        var result = _validator.TestValidate(
            new UpdateRoomCommand(Guid.CreateVersion7(), label, Floor: 1, Seats: 10)
        );

        result.ShouldHaveValidationErrorFor(x => x.Label);
    }

    [Test]
    public void GivenLabelOf64Chars_WhenValidating_ThenShouldNotHaveError()
    {
        var label = new string('А', 64);

        var result = _validator.TestValidate(
            new UpdateRoomCommand(Guid.CreateVersion7(), label, Floor: 1, Seats: 10)
        );

        result.ShouldNotHaveValidationErrorFor(x => x.Label);
    }

    [Test]
    [Arguments((short)0)]
    [Arguments((short)-1)]
    [Arguments((short)201)]
    public void GivenInvalidSeats_WhenValidating_ThenShouldHaveError(short seats)
    {
        var result = _validator.TestValidate(
            new UpdateRoomCommand(Guid.CreateVersion7(), "Зал А", Floor: 1, Seats: seats)
        );

        result.ShouldHaveValidationErrorFor(x => x.Seats);
    }

    [Test]
    [Arguments((short)1)]
    [Arguments((short)200)]
    public void GivenBoundarySeats_WhenValidating_ThenShouldNotHaveError(short seats)
    {
        var result = _validator.TestValidate(
            new UpdateRoomCommand(Guid.CreateVersion7(), "Зал А", Floor: 1, Seats: seats)
        );

        result.ShouldNotHaveValidationErrorFor(x => x.Seats);
    }
}
