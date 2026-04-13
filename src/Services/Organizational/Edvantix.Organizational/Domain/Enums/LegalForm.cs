using System.ComponentModel.DataAnnotations;

namespace Edvantix.Organizational.Domain.Enums;

/// <summary>Организационно-правовая форма юридического или физического лица.</summary>
public enum LegalForm
{
    /// <summary>Общество с ограниченной ответственностью.</summary>
    [Display(Name = "ООО")]
    Llc = 0,

    /// <summary>Открытое акционерное общество.</summary>
    [Display(Name = "ОАО")]
    Ojsc = 1,

    /// <summary>Закрытое акционерное общество.</summary>
    [Display(Name = "ЗАО")]
    Cjsc = 2,

    /// <summary>Унитарное предприятие.</summary>
    [Display(Name = "УП")]
    Ue = 3,

    /// <summary>Частное унитарное предприятие.</summary>
    [Display(Name = "ЧУП")]
    Pue = 4,

    /// <summary>Индивидуальный предприниматель.</summary>
    [Display(Name = "ИП")]
    IndividualEntrepreneur = 5,

    /// <summary>Производственный кооператив.</summary>
    [Display(Name = "Кооператив")]
    ProductionCooperative = 6,

    /// <summary>Государственное учреждение образования.</summary>
    [Display(Name = "ГУО")]
    StateEducationalInstitution = 7,

    /// <summary>Частное учреждение образования.</summary>
    [Display(Name = "ЧУО")]
    PrivateEducationalInstitution = 8,

    /// <summary>Общее образовательное учреждение.</summary>
    [Display(Name = "Образовательное учреждение")]
    EducationalInstitution = 9,
}
