using Edvantix.Organizational.Grpc.Services;
using Edvantix.Organizational.Grpc.Services.Permissions;
using Edvantix.Organizational.Pipelines;
using Edvantix.Organizational.UnitTests.Grpc.Context;
using Grpc.Core;

namespace Edvantix.Organizational.UnitTests.Grpc.Services;

public sealed class PermissionServiceTests
{
    private readonly Mock<IPermissionChecker> _checkerMock = new();

    private PermissionService CreateService() => new(_checkerMock.Object);

    private static TestServerCallContext CreateContext() => new();

    // ─── CheckPermission ───────────────────────────────────────────────────────

    [Test]
    public async Task GivenInvalidOrganizationId_WhenCheckPermission_ThenShouldThrowRpcInvalidArgument()
    {
        var request = new CheckPermissionRequest
        {
            OrganizationId = "not-a-guid",
            ProfileId = Guid.CreateVersion7().ToString(),
            Permission = "Organization.View",
        };

        var ex = await Should.ThrowAsync<RpcException>(() =>
            CreateService().CheckPermission(request, CreateContext())
        );
        ex.StatusCode.ShouldBe(StatusCode.InvalidArgument);
    }

    [Test]
    public async Task GivenInvalidProfileId_WhenCheckPermission_ThenShouldThrowRpcInvalidArgument()
    {
        var request = new CheckPermissionRequest
        {
            OrganizationId = Guid.CreateVersion7().ToString(),
            ProfileId = "not-a-guid",
            Permission = "Organization.View",
        };

        var ex = await Should.ThrowAsync<RpcException>(() =>
            CreateService().CheckPermission(request, CreateContext())
        );
        ex.StatusCode.ShouldBe(StatusCode.InvalidArgument);
    }

    [Test]
    public async Task GivenEmptyPermission_WhenCheckPermission_ThenShouldThrowRpcInvalidArgument()
    {
        var request = new CheckPermissionRequest
        {
            OrganizationId = Guid.CreateVersion7().ToString(),
            ProfileId = Guid.CreateVersion7().ToString(),
            Permission = "   ",
        };

        var ex = await Should.ThrowAsync<RpcException>(() =>
            CreateService().CheckPermission(request, CreateContext())
        );
        ex.StatusCode.ShouldBe(StatusCode.InvalidArgument);
    }

    [Test]
    public async Task GivenCheckerReturnsNull_WhenCheckPermission_ThenHasPermissionShouldBeFalse()
    {
        var orgId = Guid.CreateVersion7();
        var profileId = Guid.CreateVersion7();
        _checkerMock
            .Setup(c =>
                c.CheckAsync(orgId, profileId, "Organization.View", It.IsAny<CancellationToken>())
            )
            .ReturnsAsync((bool?)null);

        var response = await CreateService()
            .CheckPermission(
                new CheckPermissionRequest
                {
                    OrganizationId = orgId.ToString(),
                    ProfileId = profileId.ToString(),
                    Permission = "Organization.View",
                },
                CreateContext()
            );

        response.HasPermission.ShouldBeFalse();
    }

    [Test]
    public async Task GivenCheckerReturnsFalse_WhenCheckPermission_ThenHasPermissionShouldBeFalse()
    {
        var orgId = Guid.CreateVersion7();
        var profileId = Guid.CreateVersion7();
        _checkerMock
            .Setup(c =>
                c.CheckAsync(orgId, profileId, "Organization.View", It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(false);

        var response = await CreateService()
            .CheckPermission(
                new CheckPermissionRequest
                {
                    OrganizationId = orgId.ToString(),
                    ProfileId = profileId.ToString(),
                    Permission = "Organization.View",
                },
                CreateContext()
            );

        response.HasPermission.ShouldBeFalse();
    }

    [Test]
    public async Task GivenCheckerReturnsTrue_WhenCheckPermission_ThenHasPermissionShouldBeTrue()
    {
        var orgId = Guid.CreateVersion7();
        var profileId = Guid.CreateVersion7();
        _checkerMock
            .Setup(c =>
                c.CheckAsync(orgId, profileId, "Organization.View", It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(true);

        var response = await CreateService()
            .CheckPermission(
                new CheckPermissionRequest
                {
                    OrganizationId = orgId.ToString(),
                    ProfileId = profileId.ToString(),
                    Permission = "Organization.View",
                },
                CreateContext()
            );

        response.HasPermission.ShouldBeTrue();
    }
}
