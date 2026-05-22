namespace Edvantix.Groups.Features.Directories.Subjects.List;

public sealed class ListSubjectsEndpoint
    : IEndpoint<Ok<PagedResult<SubjectListItemDto>>, ListSubjectsQuery, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/directories/subjects",
                async (
                    [AsParameters] ListSubjectsQuery request,
                    ISender sender,
                    CancellationToken cancellationToken
                ) => await HandleAsync(request, sender, cancellationToken)
            )
            .WithName("ListSubjects")
            .WithTags("Предметы")
            .WithSummary("Получить список предметов организации")
            .Produces<PagedResult<SubjectListItemDto>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<Ok<PagedResult<SubjectListItemDto>>> HandleAsync(
        ListSubjectsQuery request,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(request, cancellationToken);

        return TypedResults.Ok(result);
    }
}
