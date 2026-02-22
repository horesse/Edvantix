namespace Edvantix.Organizational.Features.InvitationFeature.Features.CreateInvitation;

/// <summary>
/// Валидатор команды создания приглашения.
/// </summary>
public sealed class CreateInvitationValidator : AbstractValidator<CreateInvitationCommand>
{
    public CreateInvitationValidator()
    {
        RuleFor(x => x)
            .Must(x => x.InviteeEmail is not null || x.InviteeProfileId.HasValue)
            .WithMessage("Необходимо указать email или идентификатор профиля приглашённого.");

        RuleFor(x => x.InviteeEmail)
            .EmailAddress()
            .When(x => x.InviteeEmail is not null)
            .WithMessage("Некорректный формат email.");

        RuleFor(x => x.Role).IsInEnum().WithMessage("Некорректная роль.");

        RuleFor(x => x.TtlDays)
            .InclusiveBetween(1, 30)
            .WithMessage("Срок действия должен быть от 1 до 30 дней.");
    }
}
