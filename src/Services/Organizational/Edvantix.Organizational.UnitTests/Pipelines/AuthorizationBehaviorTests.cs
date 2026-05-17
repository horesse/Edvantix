using Edvantix.Chassis.CQRS;
using Edvantix.Chassis.Security.Keycloak;
using Edvantix.Organizational.Pipelines;
using Microsoft.Extensions.Logging.Abstractions;

namespace Edvantix.Organizational.UnitTests.Pipelines;

[RequirePermission(AuthorizationBehaviorTests.TestPermission)]
internal sealed record TestCommandWithPermission : ICommand<Guid>;

internal sealed record TestCommandWithoutPermission : ICommand<Guid>;

public sealed class AuthorizationBehaviorTests
{
    internal const string TestPermission = "organizations.manage";

    private readonly Mock<ITenantContext> _tenantContextMock = new();
    private readonly Mock<IPermissionChecker> _checkerMock = new();
    private static readonly ILogger<AuthorizationBehavior<TestCommandWithPermission, Guid>> Logger =
        NullLogger<AuthorizationBehavior<TestCommandWithPermission, Guid>>.Instance;

    private static readonly Guid OrgId = Guid.CreateVersion7();
    private static readonly Guid ProfileId = Guid.CreateVersion7();

    [Test]
    public async Task GivenMessageWithoutRequirePermissionAttribute_WhenHandling_ThenShouldCallNext()
    {
        var behavior = new AuthorizationBehavior<TestCommandWithoutPermission, Guid>(
            BuildClaims(ProfileId),
            _tenantContextMock.Object,
            _checkerMock.Object,
            NullLogger<AuthorizationBehavior<TestCommandWithoutPermission, Guid>>.Instance
        );

        var nextCalled = false;
        await behavior.Handle(
            new TestCommandWithoutPermission(),
            (_, _) =>
            {
                nextCalled = true;
                return ValueTask.FromResult(Guid.Empty);
            },
            CancellationToken.None
        );

        nextCalled.ShouldBeTrue();
        _checkerMock.Verify(
            c =>
                c.CheckAsync(
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
            BuildBehavior(ProfileId)
                .Handle(
                    new TestCommandWithPermission(),
                    (_, _) => ValueTask.FromResult(Guid.Empty),
                    CancellationToken.None
                )
                .AsTask()
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
            _checkerMock.Object,
            Logger
        );

        await Should.ThrowAsync<Exception>(() =>
            behavior
                .Handle(
                    new TestCommandWithPermission(),
                    (_, _) => ValueTask.FromResult(Guid.Empty),
                    CancellationToken.None
                )
                .AsTask()
        );
    }

    [Test]
    public async Task GivenCheckerReturnsNull_WhenHandling_ThenShouldThrowForbiddenException()
    {
        SetupTenant();
        _checkerMock
            .Setup(c =>
                c.CheckAsync(OrgId, ProfileId, TestPermission, It.IsAny<CancellationToken>())
            )
            .ReturnsAsync((bool?)null);

        await Should.ThrowAsync<ForbiddenException>(() =>
            BuildBehavior(ProfileId)
                .Handle(
                    new TestCommandWithPermission(),
                    (_, _) => ValueTask.FromResult(Guid.Empty),
                    CancellationToken.None
                )
                .AsTask()
        );
    }

    [Test]
    public async Task GivenCheckerReturnsFalse_WhenHandling_ThenShouldThrowForbiddenException()
    {
        SetupTenant();
        _checkerMock
            .Setup(c =>
                c.CheckAsync(OrgId, ProfileId, TestPermission, It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(false);

        await Should.ThrowAsync<ForbiddenException>(() =>
            BuildBehavior(ProfileId)
                .Handle(
                    new TestCommandWithPermission(),
                    (_, _) => ValueTask.FromResult(Guid.Empty),
                    CancellationToken.None
                )
                .AsTask()
        );
    }

    [Test]
    public async Task GivenCheckerReturnsTrue_WhenHandling_ThenShouldCallNext()
    {
        SetupTenant();
        _checkerMock
            .Setup(c =>
                c.CheckAsync(OrgId, ProfileId, TestPermission, It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(true);

        var nextCalled = false;
        await BuildBehavior(ProfileId)
            .Handle(
                new TestCommandWithPermission(),
                (_, _) =>
                {
                    nextCalled = true;
                    return ValueTask.FromResult(Guid.Empty);
                },
                CancellationToken.None
            );

        nextCalled.ShouldBeTrue();
    }

    [Test]
    public async Task GivenCheckerReturnsTrue_WhenHandling_ThenCheckerShouldReceiveCorrectOrgAndProfile()
    {
        SetupTenant();
        Guid? capturedOrg = null;
        Guid? capturedProfile = null;

        _checkerMock
            .Setup(c =>
                c.CheckAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<Guid>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Callback<Guid, Guid, string, CancellationToken>(
                (org, profile, _, _) =>
                {
                    capturedOrg = org;
                    capturedProfile = profile;
                }
            )
            .ReturnsAsync(true);

        await BuildBehavior(ProfileId)
            .Handle(
                new TestCommandWithPermission(),
                (_, _) => ValueTask.FromResult(Guid.Empty),
                CancellationToken.None
            );

        capturedOrg.ShouldBe(OrgId);
        capturedProfile.ShouldBe(ProfileId);
    }

    private void SetupTenant()
    {
        _tenantContextMock.Setup(t => t.IsResolved).Returns(true);
        _tenantContextMock.Setup(t => t.OrganizationId).Returns(OrgId);
    }

    private AuthorizationBehavior<TestCommandWithPermission, Guid> BuildBehavior(Guid profileId) =>
        new(BuildClaims(profileId), _tenantContextMock.Object, _checkerMock.Object, Logger);

    private static ClaimsPrincipal BuildClaims(Guid profileId) =>
        new(new ClaimsIdentity([new Claim(KeycloakClaimTypes.Profile, profileId.ToString())]));
}
