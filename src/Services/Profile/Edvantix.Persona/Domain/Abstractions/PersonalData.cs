namespace Edvantix.Persona.Domain.Abstractions;

/// <summary>
/// Базовый класс для сущностей, принадлежащих агрегату Profile.
/// ProfileId устанавливается автоматически через навигационные свойства EF Core.
/// </summary>
public abstract class PersonalData : Entity
{
    public Guid ProfileId { get; protected set; }
    public Profile Profile { get; protected set; } = null!;
}
