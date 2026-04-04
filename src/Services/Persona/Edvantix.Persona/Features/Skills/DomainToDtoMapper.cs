namespace Edvantix.Persona.Features.Skills;

public sealed class DomainToDtoMapper : Mapper<ProfileSkill, SkillDto>
{
    public override SkillDto Map(ProfileSkill source) => new(source.Skill.Id, source.Skill.Name);
}
