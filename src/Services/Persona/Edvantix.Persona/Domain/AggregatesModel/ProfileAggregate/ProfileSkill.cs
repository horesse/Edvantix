using Edvantix.Persona.Domain.AggregatesModel.SkillAggregate;

namespace Edvantix.Persona.Domain.AggregatesModel.ProfileAggregate;

/// <summary>
/// Связь профиля пользователя с навыком из глобального каталога.
/// Создаётся при добавлении навыка в профиль, удаляется при замене списка навыков.
/// </summary>
public sealed class ProfileSkill() : PersonalData
{
    /// <summary>Создаёт связь профиля с навыком по ID.</summary>
    internal ProfileSkill(Guid skillId)
        : this()
    {
        SkillId = skillId;
    }

    /// <summary>ID навыка в глобальном каталоге.</summary>
    public Guid SkillId { get; private set; }

    /// <summary>Навык из глобального каталога (навигационное свойство EF Core).</summary>
    public Skill Skill { get; private set; } = null!;
}
