namespace Edvantix.Organizational.Features.Directories.LessonTypes.Create;

/// <summary>POST /api/v1/directories/lesson-types — создать тип занятия.</summary>
public sealed class CreateLessonTypeEndpoint
    : IEndpoint<Created<LessonTypeDto>, CreateLessonTypeCommand, ISender, LinkGenerator>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/directories/lesson-types",
                async (
                    CreateLessonTypeRequest request,
                    ISender sender,
                    ITenantContext tenantContext,
                    LinkGenerator linker,
                    CancellationToken cancellationToken
                ) =>
                {
                    var command = new CreateLessonTypeCommand(
                        tenantContext.OrganizationId,
                        request.Name,
                        request.Code,
                        request.DefaultDurationMinutes,
                        request.Color,
                        request.Icon,
                        request.Order
                    );

                    return await HandleAsync(command, sender, linker, cancellationToken);
                }
            )
            .WithName("CreateLessonType")
            .WithTags("Справочник: Типы занятий")
            .WithSummary("Создать тип занятия в справочнике организации")
            .Produces<LessonTypeDto>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status422UnprocessableEntity)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<Created<LessonTypeDto>> HandleAsync(
        CreateLessonTypeCommand command,
        ISender sender,
        LinkGenerator linker,
        CancellationToken cancellationToken = default
    )
    {
        var dto = await sender.Send(command, cancellationToken);
        var location =
            linker.GetPathByName("GetLessonTypeById", new { id = dto.Id })
            ?? $"/api/v1/directories/lesson-types/{dto.Id}";

        return TypedResults.Created(location, dto);
    }
}
