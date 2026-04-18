namespace Edvantix.Organizational.Features.OrganizationMembers.Create;

internal sealed class CreateOrganizationMemberValidator
    : AbstractValidator<CreateOrganizationMemberCommand>
{
    public CreateOrganizationMemberValidator()
    {
        RuleFor(x => x.ProfileId).NotEmpty().WithMessage("Идентификатор профиля обязателен");

        RuleFor(x => x.OrganizationMemberRoleId)
            .NotEmpty()
            .WithMessage("Идентификатор роли обязателен");

        RuleFor(x => x.StartDate)
            .NotEmpty()
            .WithMessage("Дата начала участия обязательна")
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("Дата начала участия не может быть в будущем");

        RuleFor(x => x.EndDate)
            .GreaterThanOrEqualTo(x => x.StartDate)
            .WithMessage("Дата окончания участия не может быть раньше даты начала")
            .When(x => x.EndDate.HasValue);
    }
}
