using Edvantix.Organizational.Domain.Enums;
using Edvantix.Organizational.Features.Invitations.Send;

namespace Edvantix.Organizational.UnitTests.Features.Invitations.Send;

public sealed class SendInvitationValidatorTests
{
    private readonly SendInvitationValidator _validator = new();

    [Test]
    public async Task GivenValidEmailCommand_WhenValidating_ThenShouldHaveNoErrors()
    {
        var command = new SendInvitationCommand(
            Type: InvitationType.Email,
            RoleId: Guid.CreateVersion7(),
            ExpiresAt: DateTime.UtcNow.AddDays(7),
            Email: "user@example.com",
            Login: null
        );

        var result = await _validator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public async Task GivenValidInAppCommand_WhenValidating_ThenShouldHaveNoErrors()
    {
        var command = new SendInvitationCommand(
            Type: InvitationType.InApp,
            RoleId: Guid.CreateVersion7(),
            ExpiresAt: DateTime.UtcNow.AddDays(7),
            Email: null,
            Login: "johndoe"
        );

        var result = await _validator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public async Task GivenEmailTypeWithoutEmail_WhenValidating_ThenShouldHaveEmailError()
    {
        var command = new SendInvitationCommand(
            Type: InvitationType.Email,
            RoleId: Guid.CreateVersion7(),
            ExpiresAt: DateTime.UtcNow.AddDays(7),
            Email: null,
            Login: null
        );

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Test]
    public async Task GivenEmailTypeWithInvalidEmail_WhenValidating_ThenShouldHaveEmailError()
    {
        var command = new SendInvitationCommand(
            Type: InvitationType.Email,
            RoleId: Guid.CreateVersion7(),
            ExpiresAt: DateTime.UtcNow.AddDays(7),
            Email: "not-an-email",
            Login: null
        );

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Test]
    public async Task GivenInAppTypeWithoutLogin_WhenValidating_ThenShouldHaveLoginError()
    {
        var command = new SendInvitationCommand(
            Type: InvitationType.InApp,
            RoleId: Guid.CreateVersion7(),
            ExpiresAt: DateTime.UtcNow.AddDays(7),
            Email: null,
            Login: null
        );

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Login);
    }

    [Test]
    public async Task GivenPastExpiresAt_WhenValidating_ThenShouldHaveExpiresAtError()
    {
        var command = new SendInvitationCommand(
            Type: InvitationType.Email,
            RoleId: Guid.CreateVersion7(),
            ExpiresAt: DateTime.UtcNow.AddSeconds(-1),
            Email: "user@example.com",
            Login: null
        );

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.ExpiresAt);
    }

    [Test]
    public async Task GivenEmptyRoleId_WhenValidating_ThenShouldHaveRoleIdError()
    {
        var command = new SendInvitationCommand(
            Type: InvitationType.Email,
            RoleId: Guid.Empty,
            ExpiresAt: DateTime.UtcNow.AddDays(7),
            Email: "user@example.com",
            Login: null
        );

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.RoleId);
    }
}
