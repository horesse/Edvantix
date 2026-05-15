using Edvantix.Organizational.Domain.AggregatesModel.LevelAggregate;

namespace Edvantix.Organizational.Features.Groups;

internal static class GroupLevelValidationExtensions
{
    /// <summary>
    /// Проверяет, что уровень существует, принадлежит текущей организации и активен.
    /// Выполняет один запрос к БД для всех трёх проверок.
    /// </summary>
    internal static IRuleBuilderOptionsConditions<
        T,
        Guid
    > MustBeActiveLevelOfCurrentOrganization<T>(
        this IRuleBuilder<T, Guid> ruleBuilder,
        ILevelRepository levelRepository,
        ITenantContext tenantContext
    ) =>
        ruleBuilder.CustomAsync(
            async (levelId, context, ct) =>
            {
                var level = await levelRepository.GetByIdAsync(levelId, ct);

                if (level is null || level.IsDeleted)
                {
                    context.AddFailure("Указанный уровень не найден");
                    return;
                }

                if (level.OrganizationId != tenantContext.OrganizationId)
                {
                    context.AddFailure("Уровень не принадлежит текущей организации");
                    return;
                }

                if (!level.IsActive)
                    context.AddFailure("Уровень неактивен и не может быть назначен группе");
            }
        );
}
