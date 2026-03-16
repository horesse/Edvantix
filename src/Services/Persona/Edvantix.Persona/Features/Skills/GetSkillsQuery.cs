namespace Edvantix.Persona.Features.Skills;

/// <summary>Модель навыка для ответа API.</summary>
public sealed record SkillDto(Guid Id, string Name);

/// <summary>GET /v1/skills — поиск навыков для автодополнения.</summary>
public sealed record GetSkillsQuery(string Query, int Limit = 20)
    : IQuery<IReadOnlyList<SkillDto>>;

public sealed class GetSkillsQueryHandler(IServiceProvider provider)
    : IQueryHandler<GetSkillsQuery, IReadOnlyList<SkillDto>>
{
    public async ValueTask<IReadOnlyList<SkillDto>> Handle(
        GetSkillsQuery query,
        CancellationToken ct
    )
    {
        var skillRepo = provider.GetRequiredService<ISkillRepository>();

        var limit = Math.Clamp(query.Limit, 1, 50);
        var spec = new SkillSearchSpec(query.Query, limit);
        var skills = await skillRepo.FindAllAsync(spec, ct);

        return [.. skills.Select(s => new SkillDto(s.Id, s.Name))];
    }
}
