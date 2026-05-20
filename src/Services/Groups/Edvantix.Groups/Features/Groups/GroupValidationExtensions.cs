using Edvantix.Groups.Domain.AggregatesModel.GroupAggregate;
using Edvantix.Groups.Domain.AggregatesModel.LevelAggregate;

namespace Edvantix.Groups.Features.Groups;

internal static class GroupValidationExtensions
{
    /// <summary>Правила валидации для названия группы.</summary>
    public static IRuleBuilderOptions<T, string> GroupNameRules<T>(
        this IRuleBuilder<T, string> rule
    ) =>
        rule.NotEmpty()
            .WithMessage("Название группы обязательно")
            .MaximumLength(100)
            .WithMessage("Название группы не может превышать 100 символов");

    /// <summary>Правила валидации для описания группы.</summary>
    public static IRuleBuilderOptions<T, string> GroupDescriptionRules<T>(
        this IRuleBuilder<T, string> rule
    ) =>
        rule.NotEmpty()
            .WithMessage("Описание группы обязательно")
            .MaximumLength(2000)
            .WithMessage("Описание группы не может превышать 2000 символов");

    /// <summary>Правила валидации для вместимости группы.</summary>
    public static IRuleBuilderOptions<T, int> GroupCapacityRules<T>(
        this IRuleBuilder<T, int> rule
    ) =>
        rule.InclusiveBetween(1, 50)
            .WithMessage("Вместимость группы должна быть от 1 до 50 участников");

    /// <summary>Проверяет, что уровень существует, активен и принадлежит текущей организации.</summary>
    public static IRuleBuilderOptions<T, Guid> MustBeActiveLevelInCurrentOrganization<T>(
        this IRuleBuilder<T, Guid> rule,
        ILevelRepository levels,
        ITenantContext tenantContext
    ) =>
        rule.MustAsync(async (id, ct) =>
        {
            var level = await levels.GetByIdAsync(id, ct);
            return level is not null
                && level.OrganizationId == tenantContext.OrganizationId
                && level.IsActive
                && !level.IsDeleted;
        })
        .WithMessage("Уровень не найден или не принадлежит текущей организации.");
}
