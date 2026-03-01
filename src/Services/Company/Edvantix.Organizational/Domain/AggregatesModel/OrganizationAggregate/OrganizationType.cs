namespace Edvantix.Organizational.Domain.AggregatesModel.OrganizationAggregate;

/// <summary>
/// Тип организации в образовательной сфере.
/// Поле информационное — используется для категоризации организаций.
/// </summary>
public enum OrganizationType
{
    /// <summary>Учреждение образования (общее название)</summary>
    EducationalInstitution = 1,

    /// <summary>Учреждение общего среднего образования</summary>
    GeneralSecondaryEducation = 2,

    /// <summary>Лицей</summary>
    Lyceum = 3,

    /// <summary>Гимназия</summary>
    Gymnasium = 4,

    /// <summary>Колледж</summary>
    College = 5,

    /// <summary>Профессионально-техническое училище</summary>
    VocationalTechnicalSchool = 6,

    /// <summary>Университет, институт</summary>
    University = 7,

    /// <summary>Учреждение дополнительного образования детей и молодёжи</summary>
    AdditionalEducationForYouth = 8,

    /// <summary>Дошкольное учреждение образования</summary>
    Preschool = 9,

    /// <summary>Частный образовательный центр</summary>
    PrivateEducationalCenter = 10,

    /// <summary>Учебный центр, обучающая компания</summary>
    TrainingCenter = 11,

    /// <summary>ООО в сфере образования</summary>
    LlcEducational = 12,

    /// <summary>Индивидуальный предприниматель</summary>
    IndividualEntrepreneur = 13,

    /// <summary>Языковая школа</summary>
    LanguageSchool = 14,

    /// <summary>IT-школа, школа программирования</summary>
    ItSchool = 15,

    /// <summary>Репетиторский центр</summary>
    TutoringCenter = 16,

    /// <summary>Онлайн-платформа</summary>
    OnlinePlatform = 17,
}
