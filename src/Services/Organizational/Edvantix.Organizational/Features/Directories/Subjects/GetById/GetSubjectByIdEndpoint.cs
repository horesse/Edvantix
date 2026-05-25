namespace Edvantix.Organizational.Features.Directories.Subjects.GetById;

/// <summary>GET /api/v1/directories/subjects/{id} — получить предмет по идентификатору.</summary>
public sealed class GetSubjectByIdEndpoint : IEndpoint<Ok<SubjectDto>, Guid, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/directories/subjects/{id:guid}",
                async (Guid id, ISender sender, CancellationToken cancellationToken) =>
                    await HandleAsync(id, sender, cancellationToken)
            )
            .WithName("GetSubjectById")
            .WithTags("Справочник: Предметы")
            .WithSummary("Получить предмет по идентификатору")
            .Produces<SubjectDto>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<Ok<SubjectDto>> HandleAsync(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(new GetSubjectByIdQuery(id), cancellationToken);

        return TypedResults.Ok(result);
    }
}
