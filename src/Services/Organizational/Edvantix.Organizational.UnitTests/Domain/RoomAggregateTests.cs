using Edvantix.Organizational.Domain.AggregatesModel.RoomAggregate;

namespace Edvantix.Organizational.UnitTests.Domain;

public sealed class RoomAggregateTests
{
    private static readonly Guid ValidOrgId = Guid.CreateVersion7();

    private static Room CreateValidRoom() =>
        new(ValidOrgId, "Каб. 204", floor: 2, seats: 20);

    [Test]
    public void GivenValidData_WhenCreatingRoom_ThenShouldInitializePropertiesCorrectly()
    {
        var room = new Room(ValidOrgId, "Каб. 204", floor: 2, seats: 20);

        room.OrganizationId.ShouldBe(ValidOrgId);
        room.Label.ShouldBe("Каб. 204");
        room.Floor.ShouldBe((short)2);
        room.Seats.ShouldBe((short)20);
        room.IsDeleted.ShouldBeFalse();
    }

    [Test]
    public void GivenEmptyOrganizationId_WhenCreatingRoom_ThenShouldThrowArgumentException()
    {
        var act = () => new Room(Guid.Empty, "Каб. 204", floor: 2, seats: 20);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("   ")]
    public void GivenNullOrWhiteSpaceLabel_WhenCreatingRoom_ThenShouldThrowArgumentException(
        string? label
    )
    {
        var act = () => new Room(ValidOrgId, label!, floor: 1, seats: 10);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    [Arguments(0)]
    [Arguments(-1)]
    [Arguments(201)]
    public void GivenInvalidSeats_WhenCreatingRoom_ThenShouldThrowArgumentOutOfRangeException(
        short seats
    )
    {
        var act = () => new Room(ValidOrgId, "Каб. 204", floor: 2, seats: seats);

        act.ShouldThrow<ArgumentOutOfRangeException>();
    }

    [Test]
    public void GivenBoundarySeatsValue_WhenCreatingRoom_ThenShouldBeValid()
    {
        var roomMin = new Room(ValidOrgId, "Кабинет 1", floor: 1, seats: 1);
        var roomMax = new Room(ValidOrgId, "Зал А", floor: 3, seats: 200);

        roomMin.Seats.ShouldBe((short)1);
        roomMax.Seats.ShouldBe((short)200);
    }

    [Test]
    public void GivenValidRoom_WhenResizing_ThenSeatsShouldUpdate()
    {
        var room = CreateValidRoom();

        room.Resize(seats: 30);

        room.Seats.ShouldBe((short)30);
    }

    [Test]
    [Arguments(0)]
    [Arguments(-5)]
    [Arguments(201)]
    public void GivenInvalidSeats_WhenResizing_ThenShouldThrowArgumentOutOfRangeException(
        short seats
    )
    {
        var room = CreateValidRoom();

        var act = () => room.Resize(seats);

        act.ShouldThrow<ArgumentOutOfRangeException>();
    }

    [Test]
    public void GivenValidRoom_WhenUpdating_ThenShouldUpdateAllProperties()
    {
        var room = CreateValidRoom();

        room.Update("Зал Б", floor: 3, seats: 50);

        room.Label.ShouldBe("Зал Б");
        room.Floor.ShouldBe((short)3);
        room.Seats.ShouldBe((short)50);
    }

    [Test]
    public void GivenLabelWithSpaces_WhenCreatingRoom_ThenLabelShouldBeTrimmed()
    {
        var room = new Room(ValidOrgId, "  Каб. 204  ", floor: 2, seats: 20);

        room.Label.ShouldBe("Каб. 204");
    }

    [Test]
    public void GivenActiveRoom_WhenDeleting_ThenIsDeletedShouldBeTrue()
    {
        var room = CreateValidRoom();

        room.Delete();

        room.IsDeleted.ShouldBeTrue();
    }
}
