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
    private readonly Mock<IHybridCache> _cacheMock = new();
    private static readonly ILogger<AuthorizationBehavior<TestCommandWithPermission, Guid>> Logger =
        NullLogger<AuthorizationBehavior<TestCommandWithPermission, Guid>>.Instance;

    private static readonly Guid OrgId = Guid.CreateVersion7();
    private static readonly Guid ProfileId = Guid.CreateVersion7();

    [Test]
    public async Task GivenMessageWithoutRequirePermissionAttribute_WhenHandling_ThenShouldCallNext()
    {
        var claims = BuildClaims(ProfileId);
        var behavior = new AuthorizationBehavior<TestCommandWithoutPermission, Guid>(
            claims,
            _tenantContextMock.Object,
            _memberRepoMock.Object,
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
        _memberRepoMock.Verify(
            r =>
                r.GetActivePermissionsAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()
                ),
            Times.Never
        );
    }

    [Test]
    public async Task GivenMessageWithPermissionAttribute_WhenTenantContextNotResolved_ThenShouldThrowForbiddenException()
    {
        _tenantContextMock.Setup(t => t.IsResolved).Returns(false);

        var behavior = BuildBehavior(ProfileId);

        await Should.ThrowAsync<ForbiddenException>(() =>
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
    public async Task GivenMessageWithPermissionAttribute_WhenProfileClaimMissing_ThenShouldThrowException()
    {
        var claimsWithNoProfile = new ClaimsPrincipal(new ClaimsIdentity());

        _tenantContextMock.Setup(t => t.IsResolved).Returns(true);
        _tenantContextMock.Setup(t => t.OrganizationId).Returns(OrgId);

        var behavior = new AuthorizationBehavior<TestCommandWithPermission, Guid>(
            claimsWithNoProfile,
            _tenantContextMock.Object,
            _memberRepoMock.Object,
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
    public async Task GivenMessageWithPermissionAttribute_WhenPermissionPresent_ThenShouldCallNext()
    {
        _tenantContextMock.Setup(t => t.IsResolved).Returns(true);
        _tenantContextMock.Setup(t => t.OrganizationId).Returns(OrgId);
        _cacheMock
            .Setup(c =>
                c.GetOrCreateAsync<HashSet<string>>(
                    It.IsAny<string>(),
                    It.IsAny<Func<CancellationToken, ValueTask<HashSet<string>>>>(),
                    It.IsAny<IEnumerable<string>?>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(new HashSet<string> { TestPermission });

        var behavior = BuildBehavior(ProfileId);
        var nextCalled = false;

        await behavior.Handle(
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
        _tenantContextMock.Setup(t => t.IsResolved).Returns(true);
        _tenantContextMock.Setup(t => t.OrganizationId).Returns(OrgId);
        _cacheMock
            .Setup(c =>
                c.GetOrCreateAsync<HashSet<string>>(
                    It.IsAny<string>(),
                    It.IsAny<Func<CancellationToken, ValueTask<HashSet<string>>>>(),
                    It.IsAny<IEnumerable<string>?>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(new HashSet<string> { "other.permission" });

        var behavior = BuildBehavior(ProfileId);

        await Should.ThrowAsync<ForbiddenException>(() =>
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
    public async Task GivenMessageWithPermissionAttribute_WhenPermissionPresent_ThenShouldUseCacheKeyWithOrgAndProfile()
    {
        _tenantContextMock.Setup(t => t.IsResolved).Returns(true);
        _tenantContextMock.Setup(t => t.OrganizationId).Returns(OrgId);

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

        var behavior = BuildBehavior(ProfileId);

        await behavior.Handle(
            new TestCommandWithPermission(),
            (_, _) => ValueTask.FromResult(Guid.Empty),
            CancellationToken.None
        );

        capturedKey.ShouldBe($"perm:org:{OrgId}:profile:{ProfileId}");
    }

    [Test]
    public async Task GivenMessageWithPermissionAttribute_WhenPermissionPresentWithDifferentCase_ThenShouldCallNext()
    {
        _tenantContextMock.Setup(t => t.IsResolved).Returns(true);
        _tenantContextMock.Setup(t => t.OrganizationId).Returns(OrgId);
        _cacheMock
            .Setup(c =>
                c.GetOrCreateAsync<HashSet<string>>(
                    It.IsAny<string>(),
                    It.IsAny<Func<CancellationToken, ValueTask<HashSet<string>>>>(),
                    It.IsAny<IEnumerable<string>?>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(new HashSet<string> { TestPermission.ToUpperInvariant() });

        var behavior = BuildBehavior(ProfileId);
        var nextCalled = false;

        await behavior.Handle(
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

    private AuthorizationBehavior<TestCommandWithPermission, Guid> BuildBehavior(Guid profileId) =>
        new(
            BuildClaims(profileId),
            _tenantContextMock.Object,
            _memberRepoMock.Object,
            _cacheMock.Object,
            Logger
        );

    private static ClaimsPrincipal BuildClaims(Guid profileId) =>
        new(new ClaimsIdentity([new Claim(KeycloakClaimTypes.Profile, profileId.ToString())]));
}
