using Edvantix.Organizations.Features.Permissions.RegisterPermissions;

namespace Edvantix.Organizations.UnitTests.Features;

/// <summary>Unit tests for <see cref="RegisterPermissionsCommandHandler"/>.</summary>
public sealed class RegisterPermissionsCommandHandlerTests
{
    private readonly Mock<IPermissionRepository> _permissionRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly RegisterPermissionsCommandHandler _handler;

    public RegisterPermissionsCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _unitOfWorkMock
            .Setup(u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _permissionRepositoryMock = new Mock<IPermissionRepository>();
        _permissionRepositoryMock.SetupGet(r => r.UnitOfWork).Returns(_unitOfWorkMock.Object);

        _handler = new RegisterPermissionsCommandHandler(_permissionRepositoryMock.Object);
    }

    [Test]
    public async Task GivenNewPermissionNames_WhenRegistering_ThenUpsertCalledAndSaved()
    {
        var names = new List<string> { "scheduling.read-slot", "scheduling.write-slot" };
        var command = new RegisterPermissionsCommand { PermissionNames = names };

        await _handler.Handle(command, CancellationToken.None);

        _permissionRepositoryMock.Verify(
            r => r.UpsertAsync(
                It.Is<IEnumerable<string>>(list => list.SequenceEqual(names)),
                It.IsAny<CancellationToken>()),
            Times.Once);
        _unitOfWorkMock.Verify(
            u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test]
    public async Task GivenDuplicateNames_WhenRegistering_ThenIdempotentUpsert()
    {
        // Duplicate names are passed as-is; idempotency is guaranteed by UpsertAsync implementation
        var names = new List<string>
        {
            "scheduling.read-slot",
            "scheduling.read-slot",
            "scheduling.write-slot",
        };
        var command = new RegisterPermissionsCommand { PermissionNames = names };

        await _handler.Handle(command, CancellationToken.None);

        // UpsertAsync must be called exactly once with the full list (no pre-filtering)
        _permissionRepositoryMock.Verify(
            r => r.UpsertAsync(
                It.Is<IEnumerable<string>>(list => list.SequenceEqual(names)),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
