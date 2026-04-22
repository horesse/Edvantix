using System.Security.Claims;
using Edvantix.Chassis.Security.Keycloak;
using Edvantix.Organizational.Domain.AggregatesModel.InvitationAggregate;
using Edvantix.Organizational.Domain.Enums;
using Edvantix.Organizational.Features.Invitations.Accept;

namespace Edvantix.Organizational.UnitTests.Features.Invitations.Accept;

public sealed class AcceptInvitationCommandHandlerTests
{
    private readonly Mock<IInvitationRepository> _repoMock = new();
    private readonly Guid _profileId = Guid.CreateVersion7();
    private readonly AcceptInvitationCommandHandler _handler;

    public AcceptInvitationCommandHandlerTests()
    {
        var claims = new ClaimsPrincipal(
            new ClaimsIdentity([new Claim(KeycloakClaimTypes.Profile, _profileId.ToString())])
        );

        _handler = new(_repoMock.Object, claims);
    }

    [Test]
    public async Task GivenValidToken_WhenAccepting_ThenShouldSaveEntities()
    {
        var token = InvitationToken.Generate();
        var invitation = BuildPendingInvitation();

        _repoMock
            .Setup(r => r.GetByTokenHashAsync(token.Hash, It.IsAny<CancellationToken>()))
            .ReturnsAsync(invitation);
        _repoMock
            .Setup(r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var act = async () =>
            await _handler.Handle(new AcceptInvitationCommand(token.Value), CancellationToken.None);

        await act.ShouldNotThrowAsync();
        _repoMock.Verify(
            r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenUnknownToken_WhenAccepting_ThenShouldThrowNotFoundException()
    {
        _repoMock
            .Setup(r => r.GetByTokenHashAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Invitation?)null);

        var act = async () =>
            await _handler.Handle(
                new AcceptInvitationCommand("nonexistent-token"),
                CancellationToken.None
            );

        await act.ShouldThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task GivenValidToken_WhenAccepting_ThenInvitationStatusShouldBeAccepted()
    {
        var token = InvitationToken.Generate();
        var invitation = BuildPendingInvitation();

        _repoMock
            .Setup(r => r.GetByTokenHashAsync(token.Hash, It.IsAny<CancellationToken>()))
            .ReturnsAsync(invitation);
        _repoMock
            .Setup(r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _handler.Handle(new AcceptInvitationCommand(token.Value), CancellationToken.None);

        invitation.Status.ShouldBe(InvitationStatus.Accepted);
    }

    private static Invitation BuildPendingInvitation() =>
        new(
            organizationId: Guid.CreateVersion7(),
            inviterProfileId: Guid.CreateVersion7(),
            roleId: Guid.CreateVersion7(),
            type: InvitationType.Email,
            expiresAt: DateTime.UtcNow.AddDays(7),
            email: "user@example.com"
        );
}
