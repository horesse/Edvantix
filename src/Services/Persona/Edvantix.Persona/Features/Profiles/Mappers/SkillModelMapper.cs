namespace Edvantix.Persona.Features.Profiles.Mappers;

/// <summary>Маппер ProfileSkill → SkillModel.</summary>
public sealed class SkillModelMapper : Mapper<ProfileSkill, SkillModel>
{
    public override SkillModel Map(ProfileSkill source) =>
        new(source.Skill.Id, source.Skill.Name);
}
