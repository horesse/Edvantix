using Edvantix.Organizational.Domain.AggregatesModel.RoomAggregate;

namespace Edvantix.Organizational.UnitTests.Features.Directories.Rooms.Update;

public sealed class UpdateRoomValidatorTests
{
    private static readonly Guid OrgId = Guid.CreateVersion7();

    private static UpdateRoomValidator CreateValidator(bool nameExists = false)
    {
        var tenantMock = new Mock<ITenantContext>();
        tenantMock.Setup(t => t.OrganizationId).Returns(OrgId);

        var repoMock = new Mock<IRoomRepository>();
        repoMock
            .Setup(r =>
                r.AnyAsync(
                    It.Is<ISpecification<Room>>(s => s is RoomUniqueNameSpecification),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(nameExists);

        var nameChecker = new RoomUniqueNameChecker(repoMock.Object);

        return new UpdateRoomValidator(nameChecker, tenantMock.Object);
    }

    [Test]
    public async Task GivenValidCommand_WhenValidating_ThenShouldBeValid()
    {
        var validator = CreateValidator();
        var command = new UpdateRoomCommand(
            Guid.CreateVersion7(),
            "Каб. 204",
            30,
            "2",
            RoomType.Classroom
        );

        var result = await validator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    [Arguments(0)]
    [Arguments(-1)]
    [Arguments(1001)]
    public async Task GivenInvalidCapacity_WhenValidating_ThenShouldFail(int capacity)
    {
        var validator = CreateValidator();
        var command = new UpdateRoomCommand(
            Guid.CreateVersion7(),
            "Каб. 204",
            capacity,
            null,
            RoomType.Classroom
        );

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(c => c.Capacity);
    }

    [Test]
    public async Task GivenFloorExceedingMaxLength_WhenValidating_ThenShouldFail()
    {
        var validator = CreateValidator();
        var floor = new string('1', Room.MaxFloorLength + 1);
        var command = new UpdateRoomCommand(
            Guid.CreateVersion7(),
            "Каб. 204",
            30,
            floor,
            RoomType.Classroom
        );

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(c => c.Floor);
    }

    [Test]
    public async Task GivenDuplicateName_WhenValidating_ThenShouldFail()
    {
        var validator = CreateValidator(nameExists: true);
        var command = new UpdateRoomCommand(
            Guid.CreateVersion7(),
            "Каб. 204",
            30,
            null,
            RoomType.Classroom
        );

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(
            OrganizationScopedLookupValidator<UpdateRoomCommand>.NameProperty
        );
    }

    [Test]
    public async Task GivenNegativeOrder_WhenValidating_ThenShouldFail()
    {
        var validator = CreateValidator();
        var command = new UpdateRoomCommand(
            Guid.CreateVersion7(),
            "Каб. 204",
            30,
            null,
            RoomType.Classroom,
            Order: -1
        );

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(c => c.Order);
    }
}
