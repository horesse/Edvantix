namespace Edvantix.Persona.UnitTests.Fakers;

public static class SkillFaker
{
    /// <summary>Creates a <see cref="Skill"/> instance via reflection since the constructor is internal.</summary>
    public static Skill Create(string name, Guid? id = null)
    {
        var skill = (Skill)Activator.CreateInstance(typeof(Skill), nonPublic: true)!;
        skill.Id = id ?? Guid.CreateVersion7();
        typeof(Skill).GetProperty(nameof(Skill.Name))!.SetValue(skill, name);

        return skill;
    }
}
