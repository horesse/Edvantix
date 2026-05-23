using System.Collections.Frozen;

namespace Edvantix.Organizational.Features.Settings.Directories;

/// <summary>
/// Каталог из 8 справочников настроек, отображаемых на странице <c>SettingsApp</c>.
/// <para>Порядок и набор фиксированы и совпадают с <c>SettingsApp.jsx::DIRECTORIES</c>.</para>
/// <para>Локализация имён/описаний предполагается через resx <c>Directories.ru.resx</c> при
/// появлении инфраструктуры локализации; пока строки заданы инлайн на русском, поскольку
/// проект использует единственный язык интерфейса.</para>
/// </summary>
public static class DirectoryCatalog
{
    /// <summary>Код справочника «Уровни».</summary>
    public const string Levels = "levels";

    /// <summary>Код справочника «Предметы».</summary>
    public const string Subjects = "subjects";

    /// <summary>Код справочника «Типы занятий».</summary>
    public const string LessonTypes = "lesson-types";

    /// <summary>Код справочника «Статусы студентов».</summary>
    public const string StudentStatuses = "student-statuses";

    /// <summary>Код справочника «Кабинеты».</summary>
    public const string Rooms = "rooms";

    /// <summary>Код справочника «Источники привлечения».</summary>
    public const string Sources = "sources";

    /// <summary>Код справочника «Способы оплаты».</summary>
    public const string PaymentMethods = "payment-methods";

    /// <summary>Код справочника «Теги студентов».</summary>
    public const string Tags = "tags";

    private static readonly DirectoryDescriptor[] OrderedDescriptors =
    [
        new(Levels, "Уровни", "Уровни обучения для групп и курсов.", "Layers", Badge: null),
        new(Subjects, "Предметы", "Учебные предметы и направления.", "BookOpen", Badge: null),
        new(
            LessonTypes,
            "Типы занятий",
            "Урок, консультация, тест, мастер-класс.",
            "CalendarDays",
            Badge: null
        ),
        new(
            StudentStatuses,
            "Статусы студентов",
            "Активный, в академе, выпускник, отчислен.",
            "UserCheck",
            Badge: "системный"
        ),
        new(Rooms, "Кабинеты", "Помещения и аудитории школы.", "Building2", Badge: null),
        new(
            Sources,
            "Источники привлечения",
            "Откуда студенты узнают о школе.",
            "Megaphone",
            Badge: null
        ),
        new(
            PaymentMethods,
            "Способы оплаты",
            "Карта, перевод, рассрочка, материнский капитал.",
            "CreditCard",
            Badge: null
        ),
        new(Tags, "Теги студентов", "Свободные метки для сегментации.", "Sparkles", Badge: null),
    ];

    private static readonly FrozenDictionary<string, DirectoryDescriptor> ByCodeIndex =
        OrderedDescriptors.ToFrozenDictionary(d => d.Code, StringComparer.Ordinal);

    /// <summary>Полный список справочников в фиксированном порядке отображения.</summary>
    public static IReadOnlyList<DirectoryDescriptor> All { get; } = OrderedDescriptors;

    /// <summary>Находит дескриптор справочника по машинному коду.</summary>
    /// <param name="code">Машинный код (kebab-case), напр. <c>levels</c>.</param>
    /// <returns>Дескриптор справочника или <c>null</c>, если код не найден.</returns>
    public static DirectoryDescriptor? FindByCode(string code) =>
        ByCodeIndex.TryGetValue(code, out var descriptor) ? descriptor : null;
}
