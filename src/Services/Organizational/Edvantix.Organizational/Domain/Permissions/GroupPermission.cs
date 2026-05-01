using System.ComponentModel.DataAnnotations;

namespace Edvantix.Organizational.Domain.Permissions;

/// <summary>
/// Разрешения функциональной области "Группы".
/// Имя типа (<c>nameof(GroupPermission)</c>) используется как код области,
/// а <see cref="DescriptionAttribute"/> на типе — как человекочитаемое название области.
/// </summary>
[Description("Группы")]
public enum GroupPermission
{
    [Display(Name = "Создание группы")]
    CREATE,

    [Display(Name = "Просмотр группы")]
    READ,

    [Display(Name = "Редактирование группы")]
    UPDATE,

    [Display(Name = "Удаление группы")]
    DELETE,

    [Display(Name = "Управление участниками группы")]
    MANAGE_MEMBERS,

    [Display(Name = "Просмотр участников группы")]
    VIEW_MEMBERS,

    [Display(Name = "Управление контентом")]
    MANAGE_CONTENT,

    [Display(Name = "Просмотр контента")]
    VIEW_CONTENT,

    [Display(Name = "Управление расписанием")]
    MANAGE_SCHEDULE,
}
