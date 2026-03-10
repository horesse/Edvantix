using Edvantix.Organizational.Domain.AggregatesModel.OrganizationCustomRoleAggregate;

namespace Edvantix.Organizational.Features.OrganizationCustomRoleFeature.Models;

/// <summary>
/// Модель системной базовой роли организации.
/// </summary>
public sealed record BaseRoleModel
{
    /// <summary>Код базовой роли (lowercase имя enum).</summary>
    public string Code { get; init; } = string.Empty;

    /// <summary>Человекочитаемое описание роли.</summary>
    public string Description { get; init; } = string.Empty;

    /// <summary>Значение enum базовой роли.</summary>
    public OrganizationBaseRole Value { get; init; }

    /// <summary>
    /// Статические описания базовых ролей.
    /// </summary>
    private static readonly IReadOnlyDictionary<OrganizationBaseRole, string> Descriptions =
        new Dictionary<OrganizationBaseRole, string>
        {
            [OrganizationBaseRole.Owner] = "Владелец организации — полный доступ ко всем функциям.",
            [OrganizationBaseRole.Admin] =
                "Администратор — управление участниками, ролями и аналитикой.",
            [OrganizationBaseRole.Manager] =
                "Менеджер — управление группами, преподавателями и базовой аналитикой.",
            [OrganizationBaseRole.Teacher] =
                "Преподаватель — управление учениками, оценками и учебными материалами.",
            [OrganizationBaseRole.Student] =
                "Студент — просмотр материалов, выполнение заданий и просмотр своих результатов.",
        };

    /// <summary>
    /// Возвращает список всех базовых ролей в виде моделей.
    /// </summary>
    public static IReadOnlyList<BaseRoleModel> All { get; } =
        Enum.GetValues<OrganizationBaseRole>()
            .Select(role => new BaseRoleModel
            {
                Code = role.ToString().ToLowerInvariant(),
                Description = Descriptions[role],
                Value = role,
            })
            .ToList()
            .AsReadOnly();
}
