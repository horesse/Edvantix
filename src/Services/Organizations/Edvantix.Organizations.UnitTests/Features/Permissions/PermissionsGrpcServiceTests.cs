using Edvantix.Organizations.Features.Permissions.CheckPermission;
using Edvantix.Organizations.Grpc.Generated;
using Mediator;
using GrpcService = Edvantix.Organizations.Grpc.Services.PermissionsGrpcService;

namespace Edvantix.Organizations.UnitTests.Features.Permissions;

/// <summary>Unit tests for <see cref="GrpcService"/>.</summary>
public sealed class PermissionsGrpcServiceTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly GrpcService _service;

    public PermissionsGrpcServiceTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _service = new GrpcService(_mediatorMock.Object);
    }

    [Test]
    public async Task GivenValidRequest_WhenGrpcCheckPermissionCalled_ThenDelegatesToMediator()
    {
        var userId = Guid.CreateVersion7();
        var schoolId = Guid.CreateVersion7();
        const string permission = "scheduling:create-slot";

        _mediatorMock
            .Setup(m =>
                m.Send(
                    It.Is<GetUserPermissionGrantQuery>(q =>
                        q.UserId == userId && q.SchoolId == schoolId && q.Permission == permission
                    ),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(true);

        var request = new CheckPermissionRequest
        {
            UserId = userId.ToString(),
            SchoolId = schoolId.ToString(),
            Permission = permission,
        };

        var reply = await _service.CheckPermission(request, null!);

        reply.HasPermission.ShouldBeTrue();
        _mediatorMock.Verify(
            m =>
                m.Send(
                    It.Is<GetUserPermissionGrantQuery>(q =>
                        q.UserId == userId && q.SchoolId == schoolId && q.Permission == permission
                    ),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
    }

    [Test]
    public async Task GivenUserLacksPermission_WhenGrpcCheckPermissionCalled_ThenReturnsHasPermissionFalse()
    {
        var userId = Guid.CreateVersion7();
        var schoolId = Guid.CreateVersion7();
        const string permission = "scheduling:create-slot";

        _mediatorMock
            .Setup(m =>
                m.Send(
                    It.IsAny<GetUserPermissionGrantQuery>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(false);

        var request = new CheckPermissionRequest
        {
            UserId = userId.ToString(),
            SchoolId = schoolId.ToString(),
            Permission = permission,
        };

        var reply = await _service.CheckPermission(request, null!);

        reply.HasPermission.ShouldBeFalse();
    }
}
