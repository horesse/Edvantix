using Edvantix.Chassis.CQRS;
using Edvantix.Chassis.Security.Keycloak;
using Edvantix.Chassis.Security.Tenant;
using Edvantix.Groups.Grpc.Services;
using Edvantix.Groups.Pipelines;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Security.Claims;

namespace Edvantix.Groups.UnitTests.Pipelines;

[RequirePermission(AuthorizationBehaviorTests.TestPermission)]
internal sealed record TestCommandWithPermission : ICommand<Guid>;

internal sealed record TestCommandWithoutPermission : ICommand<Guid>;

public sealed class AuthorizationBehaviorTests
{
    internal const string TestPermission = "Group.Manage";

    private readonly Mock<ITenantContext> _tenantContextMock = new();
    private readonly Mock<IPermissionService> _permissionServiceMock = new();
    private static readonly ILogger<AuthorizationBehavior<TestCommandWithPermission, Guid>> Logger =
        NullLogger<AuthorizationBehavior<TestCommandWithPermission, Guid>>.Instance;

    private static readonly Guid OrgId = Guid.CreateVersion7();
    private static readonly Guid ProfileId = Guid.CreateVersion7();

    [Test]
    public async Task GivenMessageWithoutRequirePermissionAttribute_WhenHandling_ThenShouldNotCallService()
    {
        var behavior = new AuthorizationBehavior<TestCommandWithoutPermission, Guid>(
            BuildClaims(ProfileId),
            _tenantContextMock.Object,
            _permissionServiceMock.Object,
            NullLogger<AuthorizationBehavior<TestCommandWithoutPermission, Guid>>.Instance
        );

        await ((IPipelineBehavior<TestCommandWithoutPermission, Guid>)behavior).Handle(
            new TestCommandWithoutPermission(),
            (_, _) => ValueTask.FromResult(Guid.Empty),
            CancellationToken.None
        );

        _permissionServiceMock.Verify(
            s =>
                s.CheckPermissionAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<Guid>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()
                ),
            Times.Never
        );
    }

    [Test]
    public async Task GivenMessageWithPermissionAttribute_WhenTenantContextNotResolved_ThenShouldThrowForbiddenException()
    {
        _tenantContextMock.Setup(t => t.IsResolved).Returns(false);

        await Should.ThrowAsync<ForbiddenException>(() =>
            InvokeBehavior(ProfileId, new TestCommandWithPermission())
        );
    }

    [Test]
    public async Task GivenMessageWithPermissionAttribute_WhenProfileClaimMissing_ThenShouldThrowException()
    {
        _tenantContextMock.Setup(t => t.IsResolved).Returns(true);
        _tenantContextMock.Setup(t => t.OrganizationId).Returns(OrgId);

        var behavior = new AuthorizationBehavior<TestCommandWithPermission, Guid>(
            new ClaimsPrincipal(new ClaimsIdentity()),
            _tenantContextMock.Object,
            _permissionServiceMock.Object,
            Logger
        );

        await Should.ThrowAsync<Exception>(() =>
            ((IPipelineBehavior<TestCommandWithPermission, Guid>)behavior)
                .Handle(
                    new TestCommandWithPermission(),
                    (_, _) => ValueTask.FromResult(Guid.Empty),
                    CancellationToken.None
                )
                .AsTask()
        );
    }

    [Test]
    public async Task GivenServiceReturnsFalse_WhenHandling_ThenShouldThrowForbiddenException()
    {
        SetupTenant();
        _permissionServiceMock
            .Setup(s =>
                s.CheckPermissionAsync(
                    OrgId,
                    ProfileId,
                    TestPermission,
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(false);

        await Should.ThrowAsync<ForbiddenException>(() =>
            InvokeBehavior(ProfileId, new TestCommandWithPermission())
        );
    }

    [Test]
    public async Task GivenServiceReturnsTrue_WhenHandling_ThenShouldNotThrow()
    {
        SetupTenant();
        _permissionServiceMock
            .Setup(s =>
                s.CheckPermissionAsync(
                    OrgId,
                    ProfileId,
                    TestPermission,
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(true);

        await InvokeBehavior(ProfileId, new TestCommandWithPermission());
    }

    [Test]
    public async Task GivenServiceReturnsTrue_WhenHandling_ThenShouldPassCorrectOrgAndProfile()
    {
        SetupTenant();
        Guid? capturedOrg = null;
        Guid? capturedProfile = null;

        _permissionServiceMock
            .Setup(s =>
                s.CheckPermissionAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<Guid>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Callback<Guid, Guid, string, CancellationToken>((org, profile, _, _) =>
            {
                capturedOrg = org;
                capturedProfile = profile;
            })
            .ReturnsAsync(true);

        await InvokeBehavior(ProfileId, new TestCommandWithPermission());

        capturedOrg.ShouldBe(OrgId);
        capturedProfile.ShouldBe(ProfileId);
    }

    [Test]
    public async Task GivenServiceReturnsTrue_WhenHandling_ThenShouldPassCorrectPermission()
    {
        SetupTenant();
        string? capturedPermission = null;

        _permissionServiceMock
            .Setup(s =>
                s.CheckPermissionAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<Guid>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Callback<Guid, Guid, string, CancellationToken>((_, _, perm, _) =>
            {
                capturedPermission = perm;
            })
            .ReturnsAsync(true);

        await InvokeBehavior(ProfileId, new TestCommandWithPermission());

        capturedPermission.ShouldBe(TestPermission);
    }

    private void SetupTenant()
    {
        _tenantContextMock.Setup(t => t.IsResolved).Returns(true);
        _tenantContextMock.Setup(t => t.OrganizationId).Returns(OrgId);
    }

    private Task InvokeBehavior(Guid profileId, TestCommandWithPermission command) =>
        ((IPipelineBehavior<TestCommandWithPermission, Guid>)BuildBehavior(profileId))
            .Handle(
                command,
                (_, _) => ValueTask.FromResult(Guid.Empty),
                CancellationToken.None
            )
            .AsTask();

    private AuthorizationBehavior<TestCommandWithPermission, Guid> BuildBehavior(Guid profileId) =>
        new(BuildClaims(profileId), _tenantContextMock.Object, _permissionServiceMock.Object, Logger);

    private static ClaimsPrincipal BuildClaims(Guid profileId) =>
        new(new ClaimsIdentity([new Claim(KeycloakClaimTypes.Profile, profileId.ToString())]));
}
