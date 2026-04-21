using Edvantix.Chassis.Caching;
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
    private readonly Mock<IOrganizationMemberRepository> _memberRepoMock = new();
    private readonly Mock<IOrganizationMemberRoleRepository> _roleRepoMock = new();
    private readonly Mock<IHybridCache> _cacheMock = new();
    private static readonly ILogger<AuthorizationBehavior<TestCommandWithPermission, Guid>> Logger =
        NullLogger<AuthorizationBehavior<TestCommandWithPermission, Guid>>.Instance;

    private static readonly Guid OrgId = Guid.CreateVersion7();
    private static readonly Guid ProfileId = Guid.CreateVersion7();
    private static readonly Guid RoleId = Guid.CreateVersion7();

    [Test]
    public async Task GivenMessageWithoutRequirePermissionAttribute_WhenHandling_ThenShouldCallNext()
    {
        var behavior = new AuthorizationBehavior<TestCommandWithoutPermission, Guid>(
            BuildClaims(ProfileId),
            _tenantContextMock.Object,
            _memberRepoMock.Object,
            _roleRepoMock.Object,
            _cacheMock.Object,
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
        _cacheMock.Verify(
            c =>
                c.GetOrCreateAsync<Guid>(
                    It.IsAny<string>(),
                    It.IsAny<Func<CancellationToken, ValueTask<Guid>>>(),
                    It.IsAny<IEnumerable<string>?>(),
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
            _memberRepoMock.Object,
            _roleRepoMock.Object,
            _cacheMock.Object,
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
    public async Task GivenMemberNotActive_WhenL1CacheReturnsEmptyGuid_ThenShouldThrowForbiddenException()
    {
        SetupTenant();
        _cacheMock
            .Setup(c =>
                c.GetOrCreateAsync<Guid>(
                    It.IsAny<string>(),
                    It.IsAny<Func<CancellationToken, ValueTask<Guid>>>(),
                    It.IsAny<IEnumerable<string>?>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(Guid.Empty);

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
    public async Task GivenMessageWithPermissionAttribute_WhenPermissionPresent_ThenShouldCallNext()
    {
        SetupActiveRoleWithPermissions(new HashSet<string> { TestPermission });

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
    public async Task GivenMessageWithPermissionAttribute_WhenPermissionAbsent_ThenShouldThrowForbiddenException()
    {
        SetupActiveRoleWithPermissions(new HashSet<string> { "other.permission" });

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
    public async Task GivenMessageWithPermissionAttribute_WhenHandling_ThenL1CacheKeyShouldContainOrgAndProfile()
    {
        SetupTenant();

        string? capturedKey = null;
        _cacheMock
            .Setup(c =>
                c.GetOrCreateAsync<Guid>(
                    It.IsAny<string>(),
                    It.IsAny<Func<CancellationToken, ValueTask<Guid>>>(),
                    It.IsAny<IEnumerable<string>?>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Callback<
                string,
                Func<CancellationToken, ValueTask<Guid>>,
                IEnumerable<string>?,
                CancellationToken
            >((key, _, _, _) => capturedKey = key)
            .ReturnsAsync(Guid.Empty);

        await Should.ThrowAsync<ForbiddenException>(() =>
            BuildBehavior(ProfileId)
                .Handle(
                    new TestCommandWithPermission(),
                    (_, _) => ValueTask.FromResult(Guid.Empty),
                    CancellationToken.None
                )
                .AsTask()
        );

        capturedKey.ShouldBe(AuthorizationCacheKeys.MemberRole(OrgId, ProfileId));
    }

    [Test]
    public async Task GivenActiveMember_WhenHandling_ThenL2CacheKeyShouldContainRoleId()
    {
        SetupTenant();
        _cacheMock
            .Setup(c =>
                c.GetOrCreateAsync<Guid>(
                    It.IsAny<string>(),
                    It.IsAny<Func<CancellationToken, ValueTask<Guid>>>(),
                    It.IsAny<IEnumerable<string>?>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(RoleId);

        string? capturedKey = null;
        _cacheMock
            .Setup(c =>
                c.GetOrCreateAsync<HashSet<string>>(
                    It.IsAny<string>(),
                    It.IsAny<Func<CancellationToken, ValueTask<HashSet<string>>>>(),
                    It.IsAny<IEnumerable<string>?>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Callback<
                string,
                Func<CancellationToken, ValueTask<HashSet<string>>>,
                IEnumerable<string>?,
                CancellationToken
            >((key, _, _, _) => capturedKey = key)
            .ReturnsAsync(new HashSet<string> { TestPermission });

        await BuildBehavior(ProfileId)
            .Handle(
                new TestCommandWithPermission(),
                (_, _) => ValueTask.FromResult(Guid.Empty),
                CancellationToken.None
            );

        capturedKey.ShouldBe(AuthorizationCacheKeys.RolePerms(RoleId));
    }

    [Test]
    public async Task GivenMessageWithPermissionAttribute_WhenPermissionPresentWithDifferentCase_ThenShouldCallNext()
    {
        SetupActiveRoleWithPermissions(new HashSet<string> { TestPermission.ToUpperInvariant() });

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

    private void SetupTenant()
    {
        _tenantContextMock.Setup(t => t.IsResolved).Returns(true);
        _tenantContextMock.Setup(t => t.OrganizationId).Returns(OrgId);
    }

    private void SetupActiveRoleWithPermissions(HashSet<string> permissions)
    {
        SetupTenant();
        _cacheMock
            .Setup(c =>
                c.GetOrCreateAsync<Guid>(
                    It.IsAny<string>(),
                    It.IsAny<Func<CancellationToken, ValueTask<Guid>>>(),
                    It.IsAny<IEnumerable<string>?>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(RoleId);
        _cacheMock
            .Setup(c =>
                c.GetOrCreateAsync<HashSet<string>>(
                    It.IsAny<string>(),
                    It.IsAny<Func<CancellationToken, ValueTask<HashSet<string>>>>(),
                    It.IsAny<IEnumerable<string>?>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(permissions);
    }

    private AuthorizationBehavior<TestCommandWithPermission, Guid> BuildBehavior(Guid profileId) =>
        new(
            BuildClaims(profileId),
            _tenantContextMock.Object,
            _memberRepoMock.Object,
            _roleRepoMock.Object,
            _cacheMock.Object,
            Logger
        );

    private static ClaimsPrincipal BuildClaims(Guid profileId) =>
        new(new ClaimsIdentity([new Claim(KeycloakClaimTypes.Profile, profileId.ToString())]));
}
