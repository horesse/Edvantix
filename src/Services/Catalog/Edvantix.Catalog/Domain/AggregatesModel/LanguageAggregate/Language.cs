namespace Edvantix.Catalog.Domain.AggregatesModel.LanguageAggregate;

/// <summary>
/// Язык по стандарту ISO 639-1. Справочная сущность, управляемая администраторами платформы.
/// </summary>
public sealed class Language() : Entity, IAggregateRoot
{
    /// <summary>
    /// Создаёт новую запись языка.
    /// </summary>
    /// <param name="code">Двухбуквенный код ISO 639-1 (например, "en", "de").</param>
    /// <param name="name">Название языка на английском (например, "English").</param>
    /// <param name="nativeName">Название языка на родном языке (например, "Deutsch" для немецкого).</param>
    public Language(string code, string name, string nativeName)
        : this()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(nativeName);

        Code = code;
        Name = name;
        NativeName = nativeName;
        IsActive = true;
    }

    /// <summary>Двухбуквенный код ISO 639-1 (например, "en", "de", "ru").</summary>
    public string Code { get; private set; } = null!;

    /// <summary>Название языка на английском (например, "English", "German").</summary>
    public string Name { get; private set; } = null!;

    /// <summary>Название языка на родном языке (например, "Deutsch", "Русский").</summary>
    public string NativeName { get; private set; } = null!;

    /// <summary>Признак активности языка в системе.</summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Обновляет изменяемые поля языка.
    /// </summary>
    /// <param name="name">Новое название на английском.</param>
    /// <param name="nativeName">Новое название на родном языке.</param>
    /// <param name="isActive">Новый статус активности.</param>
    public void Update(string name, string nativeName, bool isActive)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(nativeName);

        Name = name;
        NativeName = nativeName;
        IsActive = isActive;
    }
}
