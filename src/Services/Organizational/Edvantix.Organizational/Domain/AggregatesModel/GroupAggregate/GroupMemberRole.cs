using System.ComponentModel.DataAnnotations;

namespace Edvantix.Organizational.Domain.AggregatesModel.GroupAggregate;

/// <summary>Роль участника внутри учебной группы.</summary>
public enum GroupMemberRole
{
    /// <summary>Ученик — основной тип участника.</summary>
    [Display(Name = "Ученик")]
    Student = 0,

    /// <summary>Преподаватель, ведущий занятия группы.</summary>
    [Display(Name = "Преподаватель")]
    Teacher = 1,

    /// <summary>Ассистент преподавателя.</summary>
    [Display(Name = "Ассистент")]
    Assistant = 2,

    /// <summary>Куратор — координирует учебный процесс без ведения занятий.</summary>
    [Display(Name = "Куратор")]
    Curator = 3,
}
