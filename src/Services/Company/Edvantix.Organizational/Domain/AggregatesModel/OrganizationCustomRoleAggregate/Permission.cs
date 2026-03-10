namespace Edvantix.Organizational.Domain.AggregatesModel.OrganizationCustomRoleAggregate;

/// <summary>
/// Права доступа (permissions) в рамках организации.
/// Формат именования: Resource + Action (соответствует концепции resource:action).
/// </summary>
public enum Permission
{
    // ─── Organization ───────────────────────────────────────────────────────────

    /// <summary>Редактирование настроек и реквизитов организации.</summary>
    OrganizationUpdate,

    /// <summary>Удаление организации.</summary>
    OrganizationDelete,

    /// <summary>Создание и управление кастомными ролями организации.</summary>
    OrganizationManageCustomRoles,

    // ─── Members ────────────────────────────────────────────────────────────────

    /// <summary>Просмотр списка участников организации.</summary>
    MemberView,

    /// <summary>Добавление и удаление участников организации.</summary>
    MemberManage,

    // ─── Roles ──────────────────────────────────────────────────────────────────

    /// <summary>
    /// Назначение ролей участникам. Область применения ограничена иерархией:
    /// актор может назначать только роли ниже своей (<see cref="IOrganizationPermissionMatrix.CanManageRole"/>).
    /// </summary>
    RoleAssign,

    // ─── Groups ─────────────────────────────────────────────────────────────────

    /// <summary>Просмотр групп организации.</summary>
    GroupView,

    /// <summary>Создание групп.</summary>
    GroupCreate,

    /// <summary>Редактирование групп.</summary>
    GroupUpdate,

    /// <summary>Удаление групп.</summary>
    GroupDelete,

    /// <summary>Назначение преподавателей в группы.</summary>
    GroupAssignTeacher,

    // ─── Analytics ──────────────────────────────────────────────────────────────

    /// <summary>Просмотр базовой отчётности (отчёты, успеваемость).</summary>
    AnalyticsViewBasic,

    /// <summary>Просмотр полной аналитики по всей организации.</summary>
    AnalyticsViewAll,

    // ─── Teaching / Learning ────────────────────────────────────────────────────

    /// <summary>Просмотр учеников своей группы.</summary>
    StudentView,

    /// <summary>Выставление оценок ученикам.</summary>
    GradeManage,

    /// <summary>Просмотр учебных материалов своих групп.</summary>
    MaterialView,

    /// <summary>Выполнение и сдача заданий.</summary>
    AssignmentSubmit,

    /// <summary>Просмотр собственных результатов и оценок.</summary>
    ResultViewOwn,
}
