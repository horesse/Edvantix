namespace Edvantix.Organizational.UnitTests.Features.Rooms;

public sealed class RoomDtoMapperTests
{
    private static readonly Guid ValidOrgId = Guid.CreateVersion7();

    private static Room CreateRoom(short seats = 20) =>
        new(ValidOrgId, "Каб. 204", floor: 2, seats: seats);

    [Test]
    public void GivenRoom_WhenMappingToRoomDto_ThenShouldMapAllFields()
    {
        var room = CreateRoom();
        var mapper = new RoomDtoMapper();

        var result = mapper.Map(room);

        result.Id.ShouldBe(room.Id);
        result.OrganizationId.ShouldBe(room.OrganizationId);
        result.Label.ShouldBe(room.Label);
        result.Floor.ShouldBe(room.Floor);
        result.Seats.ShouldBe(room.Seats);
    }

    [Test]
    public void GivenRoom_WhenMappingToRoomDto_ThenFitsTightShouldDefaultToFalse()
    {
        var room = CreateRoom();
        var mapper = new RoomDtoMapper();

        var result = mapper.Map(room);

        result.FitsTight.ShouldBeFalse();
    }

    [Test]
    public void GivenRoomWithMaxSeats_WhenMappingToRoomDto_ThenSeatsShouldBePreserved()
    {
        var room = CreateRoom(seats: 200);
        var mapper = new RoomDtoMapper();

        var result = mapper.Map(room);

        result.Seats.ShouldBe((short)200);
    }
}
