using Edvantix.Organizational.Domain.AggregatesModel.RoomAggregate;

namespace Edvantix.Organizational.UnitTests.Features.Directories.Rooms.Create;

public sealed class CreateRoomValidatorTests
{
    private static readonly Guid OrgId = Guid.CreateVersion7();

    private static (CreateRoomValidator validator, Mock<IRoomRepository> repoMock) CreateValidator(
        bool nameExists = false
    )
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
        var validator = new CreateRoomValidator(nameChecker, tenantMock.Object);

        return (validator, repoMock);
    }

    [Test]
    public async Task GivenValidCommand_WhenValidating_ThenShouldBeValid()
    {
        var (validator, _) = CreateValidator();
        var command = new CreateRoomCommand("Каб. 204", 30, "2", RoomType.Classroom);

        var result = await validator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("   ")]
    public async Task GivenEmptyName_WhenValidating_ThenShouldFailOnName(string? name)
    {
        var (validator, _) = CreateValidator();
        var command = new CreateRoomCommand(name!, 30, null, RoomType.Classroom);

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(
            OrganizationScopedLookupValidator<CreateRoomCommand>.NameProperty
        );
    }

    [Test]
    public async Task GivenNameLongerThan120_WhenValidating_ThenShouldFail()
    {
        var (validator, _) = CreateValidator();
        var command = new CreateRoomCommand(new string('А', 121), 30, null, RoomType.Classroom);

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(
            OrganizationScopedLookupValidator<CreateRoomCommand>.NameProperty
        );
    }

    [Test]
    public async Task GivenDuplicateName_WhenValidating_ThenShouldFail()
    {
        var (validator, _) = CreateValidator(nameExists: true);
        var command = new CreateRoomCommand("Каб. 204", 30, null, RoomType.Classroom);

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(
            OrganizationScopedLookupValidator<CreateRoomCommand>.NameProperty
        );
    }

    [Test]
    [Arguments(0)]
    [Arguments(-1)]
    [Arguments(1001)]
    public async Task GivenInvalidCapacity_WhenValidating_ThenShouldFail(int capacity)
    {
        var (validator, _) = CreateValidator();
        var command = new CreateRoomCommand("Каб. 204", capacity, null, RoomType.Classroom);

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(c => c.Capacity);
    }

    [Test]
    [Arguments(1)]
    [Arguments(1000)]
    public async Task GivenBoundaryCapacity_WhenValidating_ThenShouldBeValid(int capacity)
    {
        var (validator, _) = CreateValidator();
        var command = new CreateRoomCommand("Каб. 204", capacity, null, RoomType.Classroom);

        var result = await validator.TestValidateAsync(command);

        result.ShouldNotHaveValidationErrorFor(c => c.Capacity);
    }

    [Test]
    public async Task GivenFloorExceedingMaxLength_WhenValidating_ThenShouldFail()
    {
        var (validator, _) = CreateValidator();
        var floor = new string('1', Room.MaxFloorLength + 1);
        var command = new CreateRoomCommand("Каб. 204", 30, floor, RoomType.Classroom);

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(c => c.Floor);
    }

    [Test]
    public async Task GivenNullFloor_WhenValidating_ThenShouldBeValid()
    {
        var (validator, _) = CreateValidator();
        var command = new CreateRoomCommand("Каб. 204", 30, null, RoomType.Classroom);

        var result = await validator.TestValidateAsync(command);

        result.ShouldNotHaveValidationErrorFor(c => c.Floor);
    }

    [Test]
    public async Task GivenNegativeOrder_WhenValidating_ThenShouldFail()
    {
        var (validator, _) = CreateValidator();
        var command = new CreateRoomCommand("Каб. 204", 30, null, RoomType.Classroom, Order: -1);

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(c => c.Order);
    }

    [Test]
    public async Task GivenZeroOrder_WhenValidating_ThenShouldBeValid()
    {
        var (validator, _) = CreateValidator();
        var command = new CreateRoomCommand("Каб. 204", 30, null, RoomType.Classroom, Order: 0);

        var result = await validator.TestValidateAsync(command);

        result.ShouldNotHaveValidationErrorFor(c => c.Order);
    }
}
