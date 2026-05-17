using Edvantix.Organizational.Domain.AggregatesModel.LevelAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.OrganizationMemberAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.RoomAggregate;

namespace Edvantix.Organizational.Features.Groups;

/// <summary>Общие правила FluentValidation для команд создания и редактирования группы.</summary>
internal static class GroupValidationExtensions
{
    /// <summary>NotEmpty + MaxLength(512) для названия группы.</summary>
    internal static IRuleBuilderOptions<T, string> GroupNameRules<T>(
        this IRuleBuilder<T, string> rule
    ) =>
        rule.NotEmpty()
            .WithMessage("Название группы обязательно")
            .MaximumLength(512)
            .WithMessage("Название группы не может превышать 512 символов");

    /// <summary>NotEmpty + MaxLength(1024) для описания группы.</summary>
    internal static IRuleBuilderOptions<T, string> GroupDescriptionRules<T>(
        this IRuleBuilder<T, string> rule
    ) =>
        rule.NotEmpty()
            .WithMessage("Описание группы обязательно")
            .MaximumLength(1024)
            .WithMessage("Описание группы не может превышать 1024 символа");

    /// <summary>InclusiveBetween(1, 50) для вместимости группы.</summary>
    internal static IRuleBuilderOptions<T, int> GroupCapacityRules<T>(
        this IRuleBuilder<T, int> rule
    ) =>
        rule.InclusiveBetween(1, 50)
            .WithMessage("Вместимость группы должна быть от 1 до 50 участников");

    /// <summary>
    /// Проверяет, что уровень существует, принадлежит текущей организации и активен.
    /// Условие <c>When(x => x.LevelId != Guid.Empty)</c> добавляется в вызывающем коде.
    /// </summary>
    internal static IRuleBuilderOptions<T, Guid> MustBeActiveLevelInCurrentOrganization<T>(
        this IRuleBuilder<T, Guid> rule,
        ILevelRepository levels,
        ITenantContext tenantContext
    ) =>
        rule.MustAsync(
                async (id, ct) =>
                    await levels.ExistsAsync(
                        id,
                        tenantContext.OrganizationId,
                        requireActive: true,
                        ct
                    )
            )
            .WithMessage("Уровень не найден или деактивирован.");

    /// <summary>
    /// Проверяет, что участник существует и принадлежит текущей организации.
    /// Условие <c>When(x => x.TeacherMemberId != Guid.Empty)</c> добавляется в вызывающем коде.
    /// </summary>
    internal static IRuleBuilderOptions<T, Guid> MustBeMemberOfCurrentOrganization<T>(
        this IRuleBuilder<T, Guid> rule,
        IOrganizationMemberRepository members,
        ITenantContext tenantContext
    ) =>
        rule.MustAsync(
                async (id, ct) => await members.ExistsAsync(id, tenantContext.OrganizationId, ct)
            )
            .WithMessage("Преподаватель не найден.");

    /// <summary>
    /// Проверяет, что кабинет существует и принадлежит текущей организации.
    /// Условие <c>When(x => x.Format != GroupFormat.Online &amp;&amp; x.RoomId.HasValue)</c>
    /// добавляется в вызывающем коде.
    /// </summary>
    internal static IRuleBuilderOptions<T, Guid?> MustExistAsRoomInCurrentOrganization<T>(
        this IRuleBuilder<T, Guid?> rule,
        IRoomRepository rooms,
        ITenantContext tenantContext
    ) =>
        rule.MustAsync(
                async (id, ct) =>
                    await rooms.ExistsAsync(id!.Value, tenantContext.OrganizationId, ct)
            )
            .WithMessage("Кабинет не найден.");
}
