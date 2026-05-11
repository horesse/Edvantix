using System.ComponentModel.DataAnnotations;

namespace Edvantix.Organizational.Domain.AggregatesModel.GroupAggregate;

/// <summary>Жизненный цикл учебной группы.</summary>
public enum GroupStatus
{
    /// <summary>Группа активна — занятия идут.</summary>
    [Display(Name = "Активна")]
    Active = 0,

    /// <summary>Группа набирает участников.</summary>
    [Display(Name = "Набор")]
    Recruiting = 1,

    /// <summary>Занятия временно приостановлены.</summary>
    [Display(Name = "Пауза")]
    Paused = 2,

    /// <summary>Курс завершён, группа закрыта.</summary>
    [Display(Name = "Завершена")]
    Finished = 3,

    /// <summary>Группа заархивирована — недоступна для изменений.</summary>
    [Display(Name = "Архив")]
    Archived = 4,
}
