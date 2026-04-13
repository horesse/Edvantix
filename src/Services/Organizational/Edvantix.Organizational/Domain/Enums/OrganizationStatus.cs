using System.ComponentModel.DataAnnotations;

namespace Edvantix.Organizational.Domain.Enums;

/// <summary>Жизненный статус организационных сущностей платформы.</summary>
public enum OrganizationStatus
{
    /// <summary>Запись активна и участвует в операционных процессах.</summary>
    [Display(Name = "Активно")]
    Active = 0,

    /// <summary>Запись временно неактивна, данные сохранены.</summary>
    [Display(Name = "Временно бездействует")]
    Archived = 1,

    /// <summary>Запись помечена как удалённая, исключается из операционных выборок.</summary>
    [Display(Name = "Бездействует, удалено")]
    Deleted = 2
}
