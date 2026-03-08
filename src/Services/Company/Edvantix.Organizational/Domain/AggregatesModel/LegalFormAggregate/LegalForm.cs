namespace Edvantix.Organizational.Domain.AggregatesModel.LegalFormAggregate;

/// <summary>
/// Организационно-правовая форма юридического лица.
/// Справочная сущность с полным и сокращённым наименованием.
/// </summary>
public sealed class LegalForm() : Entity
{
    /// <summary>
    /// Инициализирует организационно-правовую форму.
    /// </summary>
    /// <param name="name">Полное наименование (например, «ООО (общество с ограниченной ответственностью)»).</param>
    /// <param name="shortName">Сокращённое наименование (например, «ООО»).</param>
    public LegalForm(string name, string shortName)
        : this()
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Наименование не может быть пустым.", nameof(name));

        if (string.IsNullOrWhiteSpace(shortName))
            throw new ArgumentException(
                "Сокращённое наименование не может быть пустым.",
                nameof(shortName)
            );

        Name = name;
        ShortName = shortName;
    }

    /// <summary>Полное наименование организационно-правовой формы.</summary>
    public string Name { get; private set; } = null!;

    /// <summary>Сокращённое наименование (аббревиатура), например «ООО», «ИП», «ГУО».</summary>
    public string ShortName { get; private set; } = null!;
}
