using System.Security.Claims;
using Edvantix.Chassis.Security.Keycloak;
using Edvantix.Organizational.Domain.AggregatesModel.InvitationAggregate;
using Edvantix.Organizational.Domain.Enums;
using Edvantix.Organizational.Features.Invitations.Send;
using Edvantix.Organizational.Grpc.Services.Profiles;
using Edvantix.Persona.Grpc.Services;

namespace Edvantix.Organizational.UnitTests.Features.Invitations.Send;

public sealed class SendInvitationCommandHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<IInvitationRepository> _repoMock = new();
    private readonly Mock<IProfileService> _profileServiceMock = new();
    private readonly Guid _organizationId = Guid.CreateVersion7();
    private readonly Guid _inviterProfileId = Guid.CreateVersion7();
    private readonly SendInvitationCommandHandler _handler;

    public SendInvitationCommandHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_organizationId);

        var claims = new ClaimsPrincipal(
            new ClaimsIdentity([
                new Claim(KeycloakClaimTypes.Profile, _inviterProfileId.ToString()),
            ])
        );

        _repoMock
            .Setup(r => r.AddAsync(It.IsAny<Invitation>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _repoMock
            .Setup(r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _handler = new(_tenantMock.Object, _repoMock.Object, _profileServiceMock.Object, claims);
    }

    [Test]
    public async Task GivenValidEmailCommand_WhenHandling_ThenShouldAddInvitationAndReturnId()
    {
        var command = BuildEmailCommand();

        var result = await _handler.Handle(command, CancellationToken.None);

        result.ShouldNotBe(Guid.Empty);
        _repoMock.Verify(
            r => r.AddAsync(It.IsAny<Invitation>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenValidEmailCommand_WhenHandling_ThenShouldSaveEntities()
    {
        var command = BuildEmailCommand();

        await _handler.Handle(command, CancellationToken.None);

        _repoMock.Verify(
            r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenInAppCommand_WhenHandling_ThenShouldCallPersonaToResolveLogin()
    {
        var inviteeProfileId = Guid.CreateVersion7();
        var inviteeAccountId = Guid.CreateVersion7();

        _profileServiceMock
            .Setup(s => s.GetProfileByLoginAsync("johndoe", It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new GetProfileByLoginResponse
                {
                    ProfileId = inviteeProfileId.ToString(),
                    AccountId = inviteeAccountId.ToString(),
                    FullName = "John Doe",
                    Login = "johndoe",
                }
            );

        var command = BuildInAppCommand("johndoe");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.ShouldNotBe(Guid.Empty);
        _profileServiceMock.Verify(
            s => s.GetProfileByLoginAsync("johndoe", It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenInAppCommandWithUnknownLogin_WhenHandling_ThenShouldThrowNotFoundException()
    {
        _profileServiceMock
            .Setup(s => s.GetProfileByLoginAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((GetProfileByLoginResponse?)null);

        var command = BuildInAppCommand("unknown_user");

        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.ShouldThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task GivenEmailCommand_WhenHandling_ThenShouldNotCallPersonaService()
    {
        var command = BuildEmailCommand();

        await _handler.Handle(command, CancellationToken.None);

        _profileServiceMock.Verify(
            s => s.GetProfileByLoginAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    private static SendInvitationCommand BuildEmailCommand() =>
        new(
            Type: InvitationType.Email,
            RoleId: Guid.CreateVersion7(),
            ExpiresAt: DateTime.UtcNow.AddDays(7),
            Email: "invitee@example.com",
            Login: null
        );

    private static SendInvitationCommand BuildInAppCommand(string login) =>
        new(
            Type: InvitationType.InApp,
            RoleId: Guid.CreateVersion7(),
            ExpiresAt: DateTime.UtcNow.AddDays(7),
            Email: null,
            Login: login
        );
}
