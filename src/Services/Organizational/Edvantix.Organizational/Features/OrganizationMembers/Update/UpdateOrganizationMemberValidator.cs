namespace Edvantix.Organizational.Features.OrganizationMembers.Update;

internal sealed class UpdateOrganizationMemberValidator
    : AbstractValidator<UpdateOrganizationMemberCommand>
{
    public UpdateOrganizationMemberValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Идентификатор участника обязателен");

        RuleFor(x => x.OrganizationMemberRoleId)
            .NotEmpty()
            .WithMessage("Идентификатор новой роли обязателен");
    }
}
