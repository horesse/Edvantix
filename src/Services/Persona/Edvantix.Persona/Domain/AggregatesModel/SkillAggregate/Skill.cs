namespace Edvantix.Persona.Domain.AggregatesModel.SkillAggregate;

/// <summary>
/// Навык из глобального каталога. Используется как справочник для автодополнения
/// и связывается с профилями через <see cref="ProfileSkill"/>.
/// Создаётся автоматически при первом использовании навыка пользователем,
/// удаляется когда ни один профиль не ссылается на него.
/// </summary>
public sealed class Skill() : Entity
{
    /// <summary>Создаёт новый навык в каталоге.</summary>
    internal Skill(string name)
        : this()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));
        Name = name.Trim();
    }

    /// <summary>Название навыка в оригинальном регистре.</summary>
    public string Name { get; private set; } = null!;
}
