namespace Edvantix.Groups.Features.Groups.Members.Add;

internal sealed class AddGroupMemberValidator : AbstractValidator<AddGroupMemberCommand>
{
    public AddGroupMemberValidator()
    {
        RuleFor(x => x.GroupId).NotEmpty().WithMessage("Идентификатор группы обязателен");

        RuleFor(x => x.ProfileId).NotEmpty().WithMessage("Идентификатор профиля обязателен");

        RuleFor(x => x.JoinedAt)
            .NotEqual(default(DateOnly))
            .WithMessage("Дата вступления обязательна");
    }
}
