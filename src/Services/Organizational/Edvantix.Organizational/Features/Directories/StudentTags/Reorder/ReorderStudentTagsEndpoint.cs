namespace Edvantix.Organizational.Features.Directories.StudentTags.Reorder;

/// <summary>PATCH /api/v1/directories/tags/reorder — переупорядочить теги студентов.</summary>
public sealed class ReorderStudentTagsEndpoint
    : IEndpoint<NoContent, ReorderStudentTagsCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch(
                "/directories/tags/reorder",
                async (
                    ReorderStudentTagsCommand command,
                    ISender sender,
                    CancellationToken cancellationToken
                ) => await HandleAsync(command, sender, cancellationToken)
            )
            .WithName("ReorderStudentTags")
            .WithTags("Справочник: Теги студентов")
            .WithSummary("Изменить порядок тегов студентов")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<NoContent> HandleAsync(
        ReorderStudentTagsCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(command, cancellationToken);

        return TypedResults.NoContent();
    }
}
