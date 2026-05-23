using Edvantix.Groups.Features.Directories.LessonTypes.GetById;

namespace Edvantix.Groups.Features.Directories.LessonTypes.Update;

/// <summary>PUT /api/v1/directories/lesson-types/{id} — обновить тип занятия.</summary>
public sealed class UpdateLessonTypeEndpoint
    : IEndpoint<Ok<LessonTypeDto>, UpdateLessonTypeCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(
                "/directories/lesson-types/{id:guid}",
                async (
                    Guid id,
                    UpdateLessonTypeRequest request,
                    ISender sender,
                    ITenantContext tenantContext,
                    CancellationToken cancellationToken
                ) =>
                {
                    var command = new UpdateLessonTypeCommand(
                        id,
                        tenantContext.OrganizationId,
                        request.Name,
                        request.Code,
                        request.DefaultDurationMinutes,
                        request.Color,
                        request.Icon,
                        request.Order
                    );

                    return await HandleAsync(command, sender, cancellationToken);
                }
            )
            .WithName("UpdateLessonType")
            .WithTags("Типы занятий")
            .WithSummary("Обновить тип занятия в справочнике организации")
            .Produces<LessonTypeDto>()
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status422UnprocessableEntity)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<Ok<LessonTypeDto>> HandleAsync(
        UpdateLessonTypeCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var dto = await sender.Send(command, cancellationToken);

        return TypedResults.Ok(dto);
    }
}
