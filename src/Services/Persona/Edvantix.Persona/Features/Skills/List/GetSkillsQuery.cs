namespace Edvantix.Persona.Features.Skills.List;

public sealed record GetSkillsQuery(string Query, int Limit = 20) : IQuery<IReadOnlyList<SkillDto>>;

public sealed class GetSkillsQueryHandler(ISkillRepository repository)
    : IQueryHandler<GetSkillsQuery, IReadOnlyList<SkillDto>>
{
    public async ValueTask<IReadOnlyList<SkillDto>> Handle(
        GetSkillsQuery query,
        CancellationToken cancellationToken
    )
    {
        var limit = Math.Clamp(query.Limit, 1, 50);
        var spec = new SkillSpecification(query.Query, limit);
        var skills = await repository.FindAllAsync(spec, cancellationToken);

        return [.. skills.Select(s => new SkillDto(s.Id, s.Name))];
    }
}
