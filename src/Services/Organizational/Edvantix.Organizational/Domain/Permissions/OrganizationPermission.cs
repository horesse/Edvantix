using System.ComponentModel.DataAnnotations;

namespace Edvantix.Organizational.Domain.Permissions;

/// <summary>
/// Разрешения функциональной области "Организация".
/// Имя типа (<c>nameof(OrganizationPermission)</c>) используется как код области,
/// а <see cref="DescriptionAttribute"/> на типе — как человекочитаемое название области.
/// </summary>
[Description("Организация")]
public enum OrganizationPermission
{
    [Display(Name = "Просмотр организации")]
    View,

    [Display(Name = "Редактирование организации")]
    Edit,

    [Display(Name = "Удаление организации")]
    Delete,

    [Display(Name = "Приглашение участников")]
    Members,

    [Display(Name = "Управление ролями")]
    Roles,

    [Display(Name = "Управление группами")]
    Groups,

    [Display(Name = "Просмотр аналитики")]
    Analytics,

    [Display(Name = "Управление подпиской")]
    Subscription,
}
