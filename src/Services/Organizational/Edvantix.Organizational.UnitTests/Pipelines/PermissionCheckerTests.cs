using Edvantix.Organizational.Domain.AggregatesModel.OrganizationRoleAggregate;
using Edvantix.Organizational.Pipelines;
using Microsoft.Extensions.Logging.Abstractions;

namespace Edvantix.Organizational.UnitTests.Pipelines;

public sealed class PermissionCheckerTests
{
    private readonly Mock<IOrganizationMemberRepository> _memberRepoMock = new();
    private readonly Mock<IOrganizationRoleRepository> _roleRepoMock = new();
    private readonly Mock<IFusionCache> _cacheMock = new();
    private static readonly ILogger<PermissionChecker> Logger =
        NullLogger<PermissionChecker>.Instance;

    private static readonly Guid OrgId = Guid.CreateVersion7();
    private static readonly Guid ProfileId = Guid.CreateVersion7();
    private static readonly Guid RoleId = Guid.CreateVersion7();
    private const string Permission = "organizations.manage";

    private PermissionChecker BuildChecker() =>
        new(_memberRepoMock.Object, _roleRepoMock.Object, _cacheMock.Object, Logger);

    // ─── Not a member ─────────────────────────────────────────────────────────

    [Test]
    public async Task GivenL1CacheReturnsEmptyGuid_WhenCheckAsync_ThenShouldReturnNull()
    {
        SetupL1Cache(Guid.Empty);

        var result = await BuildChecker()
            .CheckAsync(OrgId, ProfileId, Permission, CancellationToken.None);

        result.ShouldBeNull();
    }

    [Test]
    public async Task GivenMemberNotFoundInRepository_WhenL1CacheFactoryInvoked_ThenShouldReturnNull()
    {
        _memberRepoMock
            .Setup(r =>
                r.GetActiveMemberRoleIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync((Guid?)null);
        SetupL1CacheCallsFactory();

        var result = await BuildChecker()
            .CheckAsync(OrgId, ProfileId, Permission, CancellationToken.None);

        result.ShouldBeNull();
    }

    // ─── No permission ────────────────────────────────────────────────────────

    [Test]
    public async Task GivenL2CacheReturnsEmptySet_WhenCheckAsync_ThenShouldReturnFalse()
    {
        SetupL1Cache(RoleId);
        SetupL2Cache([]);

        var result = await BuildChecker()
            .CheckAsync(OrgId, ProfileId, Permission, CancellationToken.None);

        result.ShouldBe(false);
    }

    [Test]
    public async Task GivenL2CacheReturnsDifferentPermission_WhenCheckAsync_ThenShouldReturnFalse()
    {
        SetupL1Cache(RoleId);
        SetupL2Cache(["other.permission"]);

        var result = await BuildChecker()
            .CheckAsync(OrgId, ProfileId, Permission, CancellationToken.None);

        result.ShouldBe(false);
    }

    // ─── Has permission ───────────────────────────────────────────────────────

    [Test]
    public async Task GivenL2CacheContainsPermission_WhenCheckAsync_ThenShouldReturnTrue()
    {
        SetupL1Cache(RoleId);
        SetupL2Cache([Permission]);

        var result = await BuildChecker()
            .CheckAsync(OrgId, ProfileId, Permission, CancellationToken.None);

        result.ShouldBe(true);
    }

    [Test]
    public async Task GivenL2CacheContainsPermissionDifferentCase_WhenCheckAsync_ThenShouldReturnTrue()
    {
        SetupL1Cache(RoleId);
        SetupL2Cache([Permission.ToUpperInvariant()]);

        var result = await BuildChecker()
            .CheckAsync(OrgId, ProfileId, Permission, CancellationToken.None);

        result.ShouldBe(true);
    }

    // ─── L1 cache key ─────────────────────────────────────────────────────────

    [Test]
    public async Task GivenCheckAsync_WhenCalled_ThenL1CacheKeyShouldContainOrgAndProfile()
    {
        string? capturedKey = null;
        _cacheMock
            .Setup(c =>
                c.GetOrSetAsync<Guid>(
                    It.IsAny<string>(),
                    It.IsAny<
                        Func<
                            FusionCacheFactoryExecutionContext<Guid>,
                            CancellationToken,
                            Task<Guid>
                        >
                    >(),
                    It.IsAny<MaybeValue<Guid>>(),
                    It.IsAny<FusionCacheEntryOptions?>(),
                    It.IsAny<IEnumerable<string>?>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Callback<
                string,
                Func<FusionCacheFactoryExecutionContext<Guid>, CancellationToken, Task<Guid>>,
                MaybeValue<Guid>,
                FusionCacheEntryOptions?,
                IEnumerable<string>?,
                CancellationToken
            >((key, _, _, _, _, _) => capturedKey = key)
            .ReturnsAsync(Guid.Empty);

        await BuildChecker().CheckAsync(OrgId, ProfileId, Permission, CancellationToken.None);

        capturedKey.ShouldBe(AuthorizationCacheKeys.MemberRole(OrgId, ProfileId));
    }

    // ─── L1 cache tags ────────────────────────────────────────────────────────

    [Test]
    public async Task GivenCheckAsync_WhenCalled_ThenL1CacheTagShouldIncludeOrgPermsTag()
    {
        IEnumerable<string>? capturedTags = null;
        _cacheMock
            .Setup(c =>
                c.GetOrSetAsync<Guid>(
                    It.IsAny<string>(),
                    It.IsAny<
                        Func<
                            FusionCacheFactoryExecutionContext<Guid>,
                            CancellationToken,
                            Task<Guid>
                        >
                    >(),
                    It.IsAny<MaybeValue<Guid>>(),
                    It.IsAny<FusionCacheEntryOptions?>(),
                    It.IsAny<IEnumerable<string>?>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Callback<
                string,
                Func<FusionCacheFactoryExecutionContext<Guid>, CancellationToken, Task<Guid>>,
                MaybeValue<Guid>,
                FusionCacheEntryOptions?,
                IEnumerable<string>?,
                CancellationToken
            >((_, _, _, _, tags, _) => capturedTags = tags)
            .ReturnsAsync(Guid.Empty);

        await BuildChecker().CheckAsync(OrgId, ProfileId, Permission, CancellationToken.None);

        capturedTags.ShouldNotBeNull();
        capturedTags.ShouldContain(AuthorizationCacheKeys.OrgPermsTag(OrgId));
    }

    // ─── L2 cache key ─────────────────────────────────────────────────────────

    [Test]
    public async Task GivenActiveMember_WhenCheckAsync_ThenL2CacheKeyShouldContainRoleId()
    {
        SetupL1Cache(RoleId);
        string? capturedKey = null;
        _cacheMock
            .Setup(c =>
                c.GetOrSetAsync<HashSet<string>>(
                    It.IsAny<string>(),
                    It.IsAny<
                        Func<
                            FusionCacheFactoryExecutionContext<HashSet<string>>,
                            CancellationToken,
                            Task<HashSet<string>>
                        >
                    >(),
                    It.IsAny<MaybeValue<HashSet<string>>>(),
                    It.IsAny<FusionCacheEntryOptions?>(),
                    It.IsAny<IEnumerable<string>?>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Callback<
                string,
                Func<
                    FusionCacheFactoryExecutionContext<HashSet<string>>,
                    CancellationToken,
                    Task<HashSet<string>>
                >,
                MaybeValue<HashSet<string>>,
                FusionCacheEntryOptions?,
                IEnumerable<string>?,
                CancellationToken
            >((key, _, _, _, _, _) => capturedKey = key)
            .ReturnsAsync(new HashSet<string> { Permission });

        await BuildChecker().CheckAsync(OrgId, ProfileId, Permission, CancellationToken.None);

        capturedKey.ShouldBe(AuthorizationCacheKeys.RolePerms(RoleId));
    }

    // ─── L2 cache tags ────────────────────────────────────────────────────────

    [Test]
    public async Task GivenActiveMember_WhenCheckAsync_ThenL2CacheTagsShouldIncludeRolePermsAndOrgPermsTag()
    {
        SetupL1Cache(RoleId);
        IEnumerable<string>? capturedTags = null;
        _cacheMock
            .Setup(c =>
                c.GetOrSetAsync<HashSet<string>>(
                    It.IsAny<string>(),
                    It.IsAny<
                        Func<
                            FusionCacheFactoryExecutionContext<HashSet<string>>,
                            CancellationToken,
                            Task<HashSet<string>>
                        >
                    >(),
                    It.IsAny<MaybeValue<HashSet<string>>>(),
                    It.IsAny<FusionCacheEntryOptions?>(),
                    It.IsAny<IEnumerable<string>?>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Callback<
                string,
                Func<
                    FusionCacheFactoryExecutionContext<HashSet<string>>,
                    CancellationToken,
                    Task<HashSet<string>>
                >,
                MaybeValue<HashSet<string>>,
                FusionCacheEntryOptions?,
                IEnumerable<string>?,
                CancellationToken
            >((_, _, _, _, tags, _) => capturedTags = tags)
            .ReturnsAsync(new HashSet<string> { Permission });

        await BuildChecker().CheckAsync(OrgId, ProfileId, Permission, CancellationToken.None);

        capturedTags.ShouldNotBeNull();
        capturedTags.ShouldContain(AuthorizationCacheKeys.RolePerms(RoleId));
        capturedTags.ShouldContain(AuthorizationCacheKeys.OrgPermsTag(OrgId));
    }

    // ─── L1 factory: member found ─────────────────────────────────────────────

    [Test]
    public async Task GivenMemberFoundInRepository_WhenL1CacheFactoryInvoked_ThenShouldReturnPermissionCheck()
    {
        _memberRepoMock
            .Setup(r =>
                r.GetActiveMemberRoleIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync((Guid?)RoleId);
        SetupL1CacheCallsFactory();
        SetupL2Cache([Permission]);

        var result = await BuildChecker()
            .CheckAsync(OrgId, ProfileId, Permission, CancellationToken.None);

        result.ShouldBe(true);
    }

    // ─── L2 factory invocation ────────────────────────────────────────────────

    [Test]
    public async Task GivenRoleNotFoundInRepository_WhenL2CacheFactoryInvoked_ThenShouldReturnFalse()
    {
        SetupL1Cache(RoleId);
        _roleRepoMock
            .Setup(r =>
                r.GetByIdWithPermissionsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync((OrganizationRole?)null);
        SetupL2CacheCallsFactory();

        var result = await BuildChecker()
            .CheckAsync(OrgId, ProfileId, Permission, CancellationToken.None);

        result.ShouldBe(false);
    }

    [Test]
    public async Task GivenRoleFoundWithPermission_WhenL2CacheFactoryInvoked_ThenShouldReturnTrue()
    {
        SetupL1Cache(RoleId);
        var role = new OrganizationRole(OrgId, "admin", "Администратор");
        role.AddPermission(new Permission("organizations", "manage", "Управление"));
        _roleRepoMock
            .Setup(r =>
                r.GetByIdWithPermissionsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(role);
        SetupL2CacheCallsFactory();

        var result = await BuildChecker()
            .CheckAsync(OrgId, ProfileId, Permission, CancellationToken.None);

        result.ShouldBe(true);
    }

    // ─── Helpers ──────────────────────────────────────────────────────────────

    private void SetupL1Cache(Guid returnValue) =>
        _cacheMock
            .Setup(c =>
                c.GetOrSetAsync<Guid>(
                    It.IsAny<string>(),
                    It.IsAny<
                        Func<
                            FusionCacheFactoryExecutionContext<Guid>,
                            CancellationToken,
                            Task<Guid>
                        >
                    >(),
                    It.IsAny<MaybeValue<Guid>>(),
                    It.IsAny<FusionCacheEntryOptions?>(),
                    It.IsAny<IEnumerable<string>?>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(returnValue);

    private void SetupL1CacheCallsFactory() =>
        _cacheMock
            .Setup(c =>
                c.GetOrSetAsync<Guid>(
                    It.IsAny<string>(),
                    It.IsAny<
                        Func<
                            FusionCacheFactoryExecutionContext<Guid>,
                            CancellationToken,
                            Task<Guid>
                        >
                    >(),
                    It.IsAny<MaybeValue<Guid>>(),
                    It.IsAny<FusionCacheEntryOptions?>(),
                    It.IsAny<IEnumerable<string>?>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Returns<
                string,
                Func<FusionCacheFactoryExecutionContext<Guid>, CancellationToken, Task<Guid>>,
                MaybeValue<Guid>,
                FusionCacheEntryOptions?,
                IEnumerable<string>?,
                CancellationToken
            >((_, factory, _, _, _, ct) => new ValueTask<Guid>(factory(null!, ct)));

    private void SetupL2Cache(HashSet<string> permissions) =>
        _cacheMock
            .Setup(c =>
                c.GetOrSetAsync<HashSet<string>>(
                    It.IsAny<string>(),
                    It.IsAny<
                        Func<
                            FusionCacheFactoryExecutionContext<HashSet<string>>,
                            CancellationToken,
                            Task<HashSet<string>>
                        >
                    >(),
                    It.IsAny<MaybeValue<HashSet<string>>>(),
                    It.IsAny<FusionCacheEntryOptions?>(),
                    It.IsAny<IEnumerable<string>?>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(permissions);

    private void SetupL2CacheCallsFactory() =>
        _cacheMock
            .Setup(c =>
                c.GetOrSetAsync<HashSet<string>>(
                    It.IsAny<string>(),
                    It.IsAny<
                        Func<
                            FusionCacheFactoryExecutionContext<HashSet<string>>,
                            CancellationToken,
                            Task<HashSet<string>>
                        >
                    >(),
                    It.IsAny<MaybeValue<HashSet<string>>>(),
                    It.IsAny<FusionCacheEntryOptions?>(),
                    It.IsAny<IEnumerable<string>?>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Returns<
                string,
                Func<
                    FusionCacheFactoryExecutionContext<HashSet<string>>,
                    CancellationToken,
                    Task<HashSet<string>>
                >,
                MaybeValue<HashSet<string>>,
                FusionCacheEntryOptions?,
                IEnumerable<string>?,
                CancellationToken
            >((_, factory, _, _, _, ct) => new ValueTask<HashSet<string>>(factory(null!, ct)));
}
