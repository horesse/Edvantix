namespace Edvantix.Persona.Domain.AggregatesModel.SkillAggregate;

public sealed class SkillSpecification : Specification<Skill>
{
    public SkillSpecification(Guid id)
    {
        Query.Where(s => s.Id == id);
    }
    
    public SkillSpecification(string query, int limit)
    {
        Query.Search(s => s.Name, $"%{query}%").OrderBy(s => s.Name).Take(limit).AsNoTracking();
    }
}
