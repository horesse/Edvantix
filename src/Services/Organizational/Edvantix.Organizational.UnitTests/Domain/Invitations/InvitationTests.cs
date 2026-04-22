using System.Security.Claims;
using Edvantix.Organizational.Domain.AggregatesModel.InvitationAggregate;
using Edvantix.Organizational.Domain.Enums;
using Edvantix.Organizational.Domain.Events;

namespace Edvantix.Organizational.UnitTests.Domain.Invitations;

public sealed class InvitationTests
{
    private static readonly Guid OrganizationId = Guid.CreateVersion7();
    private static readonly Guid InviterProfileId = Guid.CreateVersion7();
    private static readonly Guid RoleId = Guid.CreateVersion7();
    private static readonly DateTime FutureDate = DateTime.UtcNow.AddDays(7);

    // ─── Construction ──────────────────────────────────────────────────────────

    [Test]
    public void GivenValidEmailParams_WhenCreatingInvitation_ThenShouldSetStatusPending()
    {
        var invitation = BuildEmailInvitation();

        invitation.Status.ShouldBe(InvitationStatus.Pending);
    }

    [Test]
    public void GivenValidEmailParams_WhenCreatingInvitation_ThenShouldGenerateNonEmptyTokenHash()
    {
        var invitation = BuildEmailInvitation();

        invitation.TokenHash.ShouldNotBeNullOrWhiteSpace();
    }

    [Test]
    public void GivenValidEmailParams_WhenCreatingInvitation_ThenShouldRegisterCreatedDomainEvent()
    {
        var invitation = BuildEmailInvitation();

        invitation.DomainEvents.ShouldContain(e => e is InvitationCreatedDomainEvent);
    }

    [Test]
    public void GivenValidInAppParams_WhenCreatingInvitation_ThenShouldSetCorrectType()
    {
        var invitation = BuildInAppInvitation();

        invitation.Type.ShouldBe(InvitationType.InApp);
    }

    [Test]
    public void GivenEmptyOrganizationId_WhenCreatingInvitation_ThenShouldThrowArgumentException()
    {
        var act = () =>
            new Invitation(
                Guid.Empty,
                InviterProfileId,
                RoleId,
                InvitationType.Email,
                FutureDate,
                email: "test@example.com"
            );

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenPastExpiresAt_WhenCreatingInvitation_ThenShouldThrowArgumentException()
    {
        var act = () =>
            new Invitation(
                OrganizationId,
                InviterProfileId,
                RoleId,
                InvitationType.Email,
                DateTime.UtcNow.AddSeconds(-1),
                email: "test@example.com"
            );

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenEmailTypeWithoutEmail_WhenCreatingInvitation_ThenShouldThrowArgumentException()
    {
        var act = () =>
            new Invitation(
                OrganizationId,
                InviterProfileId,
                RoleId,
                InvitationType.Email,
                FutureDate,
                email: null
            );

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenInAppTypeWithoutLogin_WhenCreatingInvitation_ThenShouldThrowArgumentException()
    {
        var act = () =>
            new Invitation(
                OrganizationId,
                InviterProfileId,
                RoleId,
                InvitationType.InApp,
                FutureDate,
                inviteeLogin: null,
                inviteeProfileId: Guid.CreateVersion7(),
                inviteeAccountId: Guid.CreateVersion7()
            );

        act.ShouldThrow<ArgumentException>();
    }

    // ─── Accept ────────────────────────────────────────────────────────────────

    [Test]
    public void GivenPendingInvitation_WhenAccepting_ThenShouldSetStatusAccepted()
    {
        var invitation = BuildEmailInvitation();
        var profileId = Guid.CreateVersion7();

        invitation.Accept(profileId);

        invitation.Status.ShouldBe(InvitationStatus.Accepted);
    }

    [Test]
    public void GivenPendingInvitation_WhenAccepting_ThenShouldSetAcceptedByProfileId()
    {
        var invitation = BuildEmailInvitation();
        var profileId = Guid.CreateVersion7();

        invitation.Accept(profileId);

        invitation.AcceptedByProfileId.ShouldBe(profileId);
    }

    [Test]
    public void GivenPendingInvitation_WhenAccepting_ThenShouldRegisterAcceptedDomainEvent()
    {
        var invitation = BuildEmailInvitation();

        invitation.Accept(Guid.CreateVersion7());

        invitation.DomainEvents.ShouldContain(e => e is InvitationAcceptedDomainEvent);
    }

    [Test]
    public void GivenAlreadyAcceptedInvitation_WhenAcceptingAgain_ThenShouldThrowInvalidOperationException()
    {
        var invitation = BuildEmailInvitation();
        invitation.Accept(Guid.CreateVersion7());

        var act = () => invitation.Accept(Guid.CreateVersion7());

        act.ShouldThrow<InvalidOperationException>();
    }

    // ─── Decline ───────────────────────────────────────────────────────────────

    [Test]
    public void GivenPendingInvitation_WhenDeclining_ThenShouldSetStatusDeclined()
    {
        var invitation = BuildEmailInvitation();

        invitation.Decline();

        invitation.Status.ShouldBe(InvitationStatus.Declined);
    }

    [Test]
    public void GivenDeclinedInvitation_WhenDecliningAgain_ThenShouldThrowInvalidOperationException()
    {
        var invitation = BuildEmailInvitation();
        invitation.Decline();

        var act = () => invitation.Decline();

        act.ShouldThrow<InvalidOperationException>();
    }

    // ─── Revoke ────────────────────────────────────────────────────────────────

    [Test]
    public void GivenPendingInvitation_WhenRevoking_ThenShouldSetStatusRevoked()
    {
        var invitation = BuildEmailInvitation();

        invitation.Revoke();

        invitation.Status.ShouldBe(InvitationStatus.Revoked);
    }

    [Test]
    public void GivenAcceptedInvitation_WhenRevoking_ThenShouldThrowInvalidOperationException()
    {
        var invitation = BuildEmailInvitation();
        invitation.Accept(Guid.CreateVersion7());

        var act = () => invitation.Revoke();

        act.ShouldThrow<InvalidOperationException>();
    }

    // ─── Token ─────────────────────────────────────────────────────────────────

    [Test]
    public void GivenTokenValue_WhenComputingHash_ThenShouldBeConsistent()
    {
        const string token = "some-secure-token-value";

        var hash1 = InvitationToken.ComputeHash(token);
        var hash2 = InvitationToken.ComputeHash(token);

        hash1.ShouldBe(hash2);
    }

    [Test]
    public void GivenGeneratedToken_WhenCheckingValue_ThenShouldNotBeEmpty()
    {
        var token = InvitationToken.Generate();

        token.Value.ShouldNotBeNullOrWhiteSpace();
        token.Hash.ShouldNotBeNullOrWhiteSpace();
    }

    [Test]
    public void GivenTwoGeneratedTokens_WhenComparing_ThenShouldBeDifferent()
    {
        var token1 = InvitationToken.Generate();
        var token2 = InvitationToken.Generate();

        token1.Value.ShouldNotBe(token2.Value);
        token1.Hash.ShouldNotBe(token2.Hash);
    }

    // ─── Helpers ───────────────────────────────────────────────────────────────

    private static Invitation BuildEmailInvitation() =>
        new(
            OrganizationId,
            InviterProfileId,
            RoleId,
            InvitationType.Email,
            FutureDate,
            email: "invitee@example.com"
        );

    private static Invitation BuildInAppInvitation() =>
        new(
            OrganizationId,
            InviterProfileId,
            RoleId,
            InvitationType.InApp,
            FutureDate,
            inviteeLogin: "johndoe",
            inviteeProfileId: Guid.CreateVersion7(),
            inviteeAccountId: Guid.CreateVersion7()
        );
}
