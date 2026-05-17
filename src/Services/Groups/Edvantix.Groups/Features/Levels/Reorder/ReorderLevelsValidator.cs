namespace Edvantix.Groups.Features.Levels.Reorder;

internal sealed class ReorderLevelsValidator : AbstractValidator<ReorderLevelsCommand>
{
    public ReorderLevelsValidator()
    {
        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage("Список уровней для переупорядочивания не может быть пустым.");

        RuleForEach(x => x.Items)
            .ChildRules(item =>
            {
                item.RuleFor(i => i.Id).NotEmpty().WithMessage("Идентификатор уровня обязателен.");

                item.RuleFor(i => i.SortOrder)
                    .GreaterThanOrEqualTo((short)0)
                    .WithMessage("Порядковый номер должен быть неотрицательным.");
            });
    }
}
