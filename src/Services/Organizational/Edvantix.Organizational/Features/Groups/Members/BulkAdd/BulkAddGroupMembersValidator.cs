namespace Edvantix.Organizational.Features.Groups.Members.BulkAdd;

internal sealed class BulkAddGroupMembersValidator : AbstractValidator<BulkAddGroupMembersCommand>
{
    public BulkAddGroupMembersValidator()
    {
        RuleFor(x => x.GroupId).NotEmpty().WithMessage("Идентификатор группы обязателен");

        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage("Список участников не может быть пустым")
            .Must(items => items.Count <= 100)
            .WithMessage("Нельзя добавить более 100 участников за один запрос");

        RuleForEach(x => x.Items)
            .ChildRules(item =>
            {
                item.RuleFor(x => x.ProfileId)
                    .NotEmpty()
                    .WithMessage("Идентификатор профиля обязателен");

                item.RuleFor(x => x.JoinedAt)
                    .NotEqual(default(DateOnly))
                    .WithMessage("Дата вступления обязательна");
            });
    }
}
