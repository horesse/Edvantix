namespace Edvantix.Persona.Features.Skills;

/// <summary>GET /v1/skills — поиск навыков для автодополнения.</summary>
public sealed class SkillsEndpoint : IEndpoint<Ok<IReadOnlyList<SkillDto>>, GetSkillsQuery, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/skills",
                async (
                    [AsParameters] GetSkillsQuery query,
                    ISender sender,
                    CancellationToken cancellationToken
                ) => await HandleAsync(query, sender, cancellationToken)
            )
            .WithName("SearchSkills")
            .WithTags("Skills")
            .WithSummary("Поиск навыков")
            .WithDescription(
                "Возвращает список навыков из глобального каталога по подстроке. Используется для автодополнения."
            )
            .Produces<IReadOnlyList<SkillDto>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<Ok<IReadOnlyList<SkillDto>>> HandleAsync(
        GetSkillsQuery query,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(query, cancellationToken);
        return TypedResults.Ok(result);
    }
}
