using System.ComponentModel.DataAnnotations;

namespace Edvantix.Organizational.Domain.Enums;

/// <summary>Тип образовательного или бизнес-учреждения организации.</summary>
public enum OrganizationType
{
    /// <summary>Учреждение образования (общее название).</summary>
    [Display(Name = "Учреждение образования")]
    EducationalInstitution = 0,

    /// <summary>Учреждение общего среднего образования.</summary>
    [Display(Name = "Учреждение общего среднего образования")]
    GeneralEducationSchool = 1,

    /// <summary>Лицей.</summary>
    [Display(Name = "Лицей")]
    Lyceum = 2,

    /// <summary>Гимназия.</summary>
    [Display(Name = "Гимназия")]
    Gymnasium = 3,

    /// <summary>Колледж.</summary>
    [Display(Name = "Колледж")]
    College = 4,

    /// <summary>Профессионально-техническое училище.</summary>
    [Display(Name = "Профессионально-техническое училище")]
    VocationalSchool = 5,

    /// <summary>Университет, институт.</summary>
    [Display(Name = "Университет, институт")]
    University = 6,

    /// <summary>Учреждение дополнительного образования детей и молодёжи.</summary>
    [Display(Name = "Учреждение дополнительного образования детей и молодёжи")]
    AdditionalEducation = 7,

    /// <summary>Дошкольное учреждение образования.</summary>
    [Display(Name = "Дошкольное учреждение образования")]
    Preschool = 8,

    /// <summary>Частный образовательный центр.</summary>
    [Display(Name = "Частный образовательный центр")]
    PrivateEducationalCenter = 9,

    /// <summary>Учебный центр, обучающая компания.</summary>
    [Display(Name = "Учебный центр, обучающая компания")]
    TrainingCompany = 10,

    /// <summary>ООО в сфере образования.</summary>
    [Display(Name = "ООО (общество с ограниченной ответственностью) в сфере образования")]
    LlcEducation = 11,

    /// <summary>Индивидуальный предприниматель.</summary>
    [Display(Name = "Индивидуальный предприниматель")]
    IndividualEntrepreneur = 12,

    /// <summary>Языковая школа.</summary>
    [Display(Name = "Языковая школа")]
    LanguageSchool = 13,

    /// <summary>IT-школа, школа программирования.</summary>
    [Display(Name = "IT-школа, школа программирования")]
    ItSchool = 14,

    /// <summary>Репетиторский центр.</summary>
    [Display(Name = "Репетиторский центр")]
    TutoringCenter = 15,

    /// <summary>Онлайн-платформа.</summary>
    [Display(Name = "Онлайн-платформа")]
    OnlinePlatform = 16
}
