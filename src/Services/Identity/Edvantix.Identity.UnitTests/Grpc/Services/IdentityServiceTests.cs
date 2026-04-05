using Edvantix.Identity.UnitTests.Grpc.Context;
using Grpc.Core;

namespace Edvantix.Identity.UnitTests.Grpc.Services;

public sealed class IdentityServiceTests
{
    private readonly Mock<IKeycloakAdminService> _keycloakMock = new();
    private readonly IdentityService _service;

    public IdentityServiceTests()
    {
        _service = new(_keycloakMock.Object, Mock.Of<ILogger<IdentityService>>());
    }

    // ─── SetProfileId ────────────────────────────────────────────────────────

    [Test]
    public async Task GivenValidRequest_WhenSetProfileIdCalled_ThenShouldDelegateToKeycloakAndReturnReply()
    {
        var accountId = Guid.CreateVersion7();
        var profileId = Guid.CreateVersion7();
        var context = new TestServerCallContext();

        var result = await _service.SetProfileId(
            new SetProfileIdRequest
            {
                AccountId = accountId.ToString(),
                ProfileId = profileId.ToString(),
            },
            context
        );

        result.ShouldNotBeNull();
        _keycloakMock.Verify(
            k => k.SetProfileIdAsync(accountId, profileId, It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenKeycloakThrows_WhenSetProfileIdCalled_ThenShouldThrowRpcExceptionWithInternalStatus()
    {
        var context = new TestServerCallContext();

        _keycloakMock
            .Setup(k =>
                k.SetProfileIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ThrowsAsync(new HttpRequestException("Keycloak unavailable"));

        var exception = await Should.ThrowAsync<RpcException>(async () =>
            await _service.SetProfileId(
                new SetProfileIdRequest
                {
                    AccountId = Guid.CreateVersion7().ToString(),
                    ProfileId = Guid.CreateVersion7().ToString(),
                },
                context
            )
        );

        exception.StatusCode.ShouldBe(StatusCode.Internal);
    }

    [Test]
    public async Task GivenInvalidGuidFormat_WhenSetProfileIdCalled_ThenShouldThrowRpcExceptionWithInternalStatus()
    {
        var context = new TestServerCallContext();

        var exception = await Should.ThrowAsync<RpcException>(async () =>
            await _service.SetProfileId(
                new SetProfileIdRequest
                {
                    AccountId = "not-a-guid",
                    ProfileId = Guid.CreateVersion7().ToString(),
                },
                context
            )
        );

        exception.StatusCode.ShouldBe(StatusCode.Internal);
    }

    // ─── SetUserEnabled (enable) ─────────────────────────────────────────────

    [Test]
    public async Task GivenEnabledTrue_WhenSetUserEnabledCalled_ThenShouldCallEnableUserAsync()
    {
        var accountId = Guid.CreateVersion7();
        var context = new TestServerCallContext();

        var result = await _service.SetUserEnabled(
            new SetUserEnabledRequest { AccountId = accountId.ToString(), Enabled = true },
            context
        );

        result.ShouldNotBeNull();
        _keycloakMock.Verify(
            k => k.EnableUserAsync(accountId, It.IsAny<CancellationToken>()),
            Times.Once
        );
        _keycloakMock.Verify(
            k => k.DisableUserAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Test]
    public async Task GivenEnabledFalse_WhenSetUserEnabledCalled_ThenShouldCallDisableUserAsync()
    {
        var accountId = Guid.CreateVersion7();
        var context = new TestServerCallContext();

        var result = await _service.SetUserEnabled(
            new SetUserEnabledRequest { AccountId = accountId.ToString(), Enabled = false },
            context
        );

        result.ShouldNotBeNull();
        _keycloakMock.Verify(
            k => k.DisableUserAsync(accountId, It.IsAny<CancellationToken>()),
            Times.Once
        );
        _keycloakMock.Verify(
            k => k.EnableUserAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Test]
    public async Task GivenKeycloakThrows_WhenSetUserEnabledCalled_ThenShouldThrowRpcExceptionWithInternalStatus()
    {
        var context = new TestServerCallContext();

        _keycloakMock
            .Setup(k => k.EnableUserAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException("Keycloak unavailable"));

        var exception = await Should.ThrowAsync<RpcException>(async () =>
            await _service.SetUserEnabled(
                new SetUserEnabledRequest
                {
                    AccountId = Guid.CreateVersion7().ToString(),
                    Enabled = true,
                },
                context
            )
        );

        exception.StatusCode.ShouldBe(StatusCode.Internal);
    }

    [Test]
    public async Task GivenInvalidGuidFormat_WhenSetUserEnabledCalled_ThenShouldThrowRpcExceptionWithInternalStatus()
    {
        var context = new TestServerCallContext();

        var exception = await Should.ThrowAsync<RpcException>(async () =>
            await _service.SetUserEnabled(
                new SetUserEnabledRequest { AccountId = "bad-guid", Enabled = true },
                context
            )
        );

        exception.StatusCode.ShouldBe(StatusCode.Internal);
    }
}
