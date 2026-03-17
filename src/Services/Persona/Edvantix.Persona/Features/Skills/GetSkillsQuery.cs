namespace Edvantix.Persona.Features.Skills;

public sealed record SkillDto(Guid Id, string Name);

public sealed record GetSkillsQuery(string Query, int Limit = 20) : IQuery<IReadOnlyList<SkillDto>>;

public sealed class GetSkillsQueryHandler(IServiceProvider provider)
    : IQueryHandler<GetSkillsQuery, IReadOnlyList<SkillDto>>
{
    public async ValueTask<IReadOnlyList<SkillDto>> Handle(
        GetSkillsQuery query,
        CancellationToken cancellationToken
    )
    {
        var skillRepo = provider.GetRequiredService<ISkillRepository>();

        var limit = Math.Clamp(query.Limit, 1, 50);
        var spec = new SkillSpecification(query.Query, limit);
        var skills = await skillRepo.FindAllAsync(spec, cancellationToken);

        return [.. skills.Select(s => new SkillDto(s.Id, s.Name))];
    }
}
