using Edvantix.Organizational.Domain.AggregatesModel.RoomAggregate;

namespace Edvantix.Organizational.UnitTests.Domain;

public sealed class RoomAggregateTests
{
    private static readonly Guid ValidOrgId = Guid.CreateVersion7();

    private static Room CreateValidRoom() =>
        new(ValidOrgId, "Каб. 204", capacity: 30, floor: "2", RoomType.Classroom);

    [Test]
    public void GivenValidData_WhenCreatingRoom_ThenShouldInitializePropertiesCorrectly()
    {
        var room = new Room(ValidOrgId, "Каб. 204", capacity: 30, floor: "2", RoomType.Classroom);

        room.OrganizationId.ShouldBe(ValidOrgId);
        room.Name.ShouldBe("Каб. 204");
        room.Capacity.ShouldBe(30);
        room.Floor.ShouldBe("2");
        room.RoomType.ShouldBe(RoomType.Classroom);
        room.IsArchived.ShouldBeFalse();
    }

    [Test]
    public void GivenNullFloor_WhenCreatingRoom_ThenFloorShouldBeNull()
    {
        var room = new Room(ValidOrgId, "Зал А", capacity: 50, floor: null, RoomType.Meeting);

        room.Floor.ShouldBeNull();
    }

    [Test]
    public void GivenFloorWithSpaces_WhenCreatingRoom_ThenFloorShouldBeTrimmed()
    {
        var room = new Room(ValidOrgId, "Зал А", capacity: 50, floor: "  3  ", RoomType.Meeting);

        room.Floor.ShouldBe("3");
    }

    [Test]
    public void GivenNameWithSpaces_WhenCreatingRoom_ThenNameShouldBeTrimmed()
    {
        var room = new Room(
            ValidOrgId,
            "  Каб. 204  ",
            capacity: 30,
            floor: null,
            RoomType.Classroom
        );

        room.Name.ShouldBe("Каб. 204");
    }

    [Test]
    public void GivenEmptyOrganizationId_WhenCreatingRoom_ThenShouldThrowArgumentException()
    {
        var act = () =>
            new Room(Guid.Empty, "Каб. 204", capacity: 30, floor: null, RoomType.Classroom);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("   ")]
    public void GivenNullOrWhiteSpaceName_WhenCreatingRoom_ThenShouldThrowArgumentException(
        string? name
    )
    {
        var act = () => new Room(ValidOrgId, name!, capacity: 30, floor: null, RoomType.Classroom);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    [Arguments(0)]
    [Arguments(-1)]
    [Arguments(1001)]
    public void GivenInvalidCapacity_WhenCreatingRoom_ThenShouldThrowArgumentOutOfRangeException(
        int capacity
    )
    {
        var act = () =>
            new Room(ValidOrgId, "Зал А", capacity: capacity, floor: null, RoomType.Classroom);

        act.ShouldThrow<ArgumentOutOfRangeException>();
    }

    [Test]
    [Arguments(1)]
    [Arguments(1000)]
    public void GivenBoundaryCapacity_WhenCreatingRoom_ThenShouldBeValid(int capacity)
    {
        var room = new Room(ValidOrgId, "Зал А", capacity: capacity, floor: null, RoomType.Classroom);

        room.Capacity.ShouldBe(capacity);
    }

    [Test]
    public void GivenFloorExceedingMaxLength_WhenCreatingRoom_ThenShouldThrowArgumentException()
    {
        var floor = new string('1', Room.MaxFloorLength + 1);
        var act = () =>
            new Room(ValidOrgId, "Зал А", capacity: 30, floor: floor, RoomType.Classroom);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenValidRoom_WhenArchiving_ThenIsArchivedShouldBeTrue()
    {
        var room = CreateValidRoom();
        var by = Guid.CreateVersion7();

        room.Archive(by);

        room.IsArchived.ShouldBeTrue();
    }

    [Test]
    public void GivenArchivedRoom_WhenArchivingAgain_ThenShouldBeIdempotent()
    {
        var room = CreateValidRoom();
        var by = Guid.CreateVersion7();
        room.Archive(by);
        var modifiedAt = room.LastModifiedAt;

        room.Archive(by);

        room.IsArchived.ShouldBeTrue();
        room.LastModifiedAt.ShouldBe(modifiedAt);
    }

    [Test]
    public void GivenArchivedRoom_WhenRestoring_ThenIsArchivedShouldBeFalse()
    {
        var room = CreateValidRoom();
        var by = Guid.CreateVersion7();
        room.Archive(by);

        room.Restore(by);

        room.IsArchived.ShouldBeFalse();
    }

    [Test]
    public void GivenActiveRoom_WhenRestoringAgain_ThenShouldBeIdempotent()
    {
        var room = CreateValidRoom();
        var by = Guid.CreateVersion7();
        var modifiedAt = room.LastModifiedAt;

        room.Restore(by);

        room.IsArchived.ShouldBeFalse();
        room.LastModifiedAt.ShouldBe(modifiedAt);
    }

    [Test]
    public void GivenValidRoom_WhenUpdating_ThenShouldUpdateAllProperties()
    {
        var room = CreateValidRoom();
        var by = Guid.CreateVersion7();

        room.Update("Лекционный зал", 200, "3", RoomType.Lab, 5, by);

        room.Name.ShouldBe("Лекционный зал");
        room.Capacity.ShouldBe(200);
        room.Floor.ShouldBe("3");
        room.RoomType.ShouldBe(RoomType.Lab);
        room.Order.ShouldBe(5);
    }

    [Test]
    public void GivenInvalidCapacityOnUpdate_WhenUpdating_ThenShouldThrow()
    {
        var room = CreateValidRoom();
        var by = Guid.CreateVersion7();

        var act = () => room.Update("Зал", 0, null, RoomType.Classroom, 0, by);

        act.ShouldThrow<ArgumentOutOfRangeException>();
    }

    [Test]
    public void GivenFloorExceedingMaxLengthOnUpdate_WhenUpdating_ThenShouldThrow()
    {
        var room = CreateValidRoom();
        var by = Guid.CreateVersion7();
        var tooLong = new string('X', Room.MaxFloorLength + 1);

        var act = () => room.Update("Зал", 30, tooLong, RoomType.Classroom, 0, by);

        act.ShouldThrow<ArgumentException>();
    }
}
