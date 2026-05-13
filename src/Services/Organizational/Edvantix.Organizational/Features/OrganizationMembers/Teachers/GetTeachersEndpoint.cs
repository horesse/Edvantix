namespace Edvantix.Organizational.Features.OrganizationMembers.Teachers;

public sealed class GetTeachersEndpoint
    : IEndpoint<Ok<IReadOnlyCollection<TeacherDto>>, GetTeachersQuery, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/members/teachers",
                async (
                    [AsParameters] GetTeachersQuery request,
                    ISender sender,
                    CancellationToken cancellationToken
                ) => await HandleAsync(request, sender, cancellationToken)
            )
            .WithName("GetTeachers")
            .WithTags("Участники организации")
            .WithSummary("Получить список преподавателей организации")
            .WithDescription(
                "Возвращает активных участников организации для выбора преподавателя группы. "
                + "Профильные данные обогащаются из сервиса Persona. "
                + "Поддерживает поиск по имени через параметр search."
            )
            .Produces<IReadOnlyCollection<TeacherDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<Ok<IReadOnlyCollection<TeacherDto>>> HandleAsync(
        GetTeachersQuery request,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(request, cancellationToken);
        return TypedResults.Ok(result);
    }
}
