using System.ComponentModel.DataAnnotations;

namespace Edvantix.Organizational.Domain.AggregatesModel.GroupAggregate;

/// <summary>Уровень сложности / целевая аудитория учебной группы.</summary>
public enum GroupLevel
{
    /// <summary>Начальный уровень — элементарное владение.</summary>
    [Display(Name = "A1")]
    A1 = 0,

    /// <summary>Ниже среднего.</summary>
    [Display(Name = "A2")]
    A2 = 1,

    /// <summary>Средний уровень.</summary>
    [Display(Name = "B1")]
    B1 = 2,

    /// <summary>Выше среднего.</summary>
    [Display(Name = "B2")]
    B2 = 3,

    /// <summary>Продвинутый уровень.</summary>
    [Display(Name = "C1")]
    C1 = 4,

    /// <summary>Младшая школа (Junior).</summary>
    [Display(Name = "Младшая школа")]
    Junior = 5,

    /// <summary>Старшая школа (Teen).</summary>
    [Display(Name = "Старшая школа")]
    Teen = 6,

    /// <summary>Подготовка (Pre-school / Preschool).</summary>
    [Display(Name = "Подготовительный")]
    Preschool = 7,
}
