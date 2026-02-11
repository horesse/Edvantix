using System.ComponentModel;

namespace Edvantix.ProfileService.Domain.AggregatesModel.EducationAggregate;

public enum EducationLevel : byte
{
    [Description("Дошкольное образование")]
    Preschool = 1,

    [Description("Общее среднее образование")]
    GeneralSecondary = 2,

    [Description("Профессионально-техническое образование")]
    VocationalTechnical = 3,

    [Description("Среднее специальное образование")]
    SecondarySpecialized = 4,

    [Description("Высшее образование (I ступень)")]
    HigherBachelor = 5,

    [Description("Высшее образование (II ступень)")]
    HigherMaster = 6,

    [Description("Послевузовское образование")]
    Postgraduate = 7,

    [Description("Дополнительное образование детей и молодежи")]
    AdditionalChildren = 8,

    [Description("Дополнительное образование взрослых")]
    AdditionalAdults = 9,

    [Description("Специальное образование")]
    Special = 10
}
