using Edvantix.Organizational.Domain.Enums;

namespace Edvantix.Organizational.Features.Invitations.Send;

internal sealed class SendInvitationValidator : AbstractValidator<SendInvitationCommand>
{
    public SendInvitationValidator()
    {
        RuleFor(x => x.RoleId).NotEmpty().WithMessage("Идентификатор роли обязателен.");

        RuleFor(x => x.ExpiresAt)
            .GreaterThan(DateTime.UtcNow)
            .WithMessage("Дата истечения должна быть в будущем.");

        When(
            x => x.Type == InvitationType.Email,
            () =>
            {
                RuleFor(x => x.Email)
                    .NotEmpty()
                    .WithMessage("Email обязателен для email-приглашения.")
                    .EmailAddress()
                    .WithMessage("Некорректный формат email.");
            }
        );

        When(
            x => x.Type == InvitationType.InApp,
            () =>
            {
                RuleFor(x => x.Login)
                    .NotEmpty()
                    .WithMessage("Логин обязателен для in-app-приглашения.")
                    .MinimumLength(3)
                    .WithMessage("Логин должен содержать не менее 3 символов.");
            }
        );
    }
}
