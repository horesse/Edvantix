namespace Edvantix.Persona.Features.Admin.Profiles.Edit;

public sealed class AdminUpdateProfileEndpoint
    : IEndpoint<NoContent, (Guid ProfileId, AdminUpdateProfileRequest Request), ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch(
                "/admin/profiles/{profileId:guid}",
                async (
                    Guid profileId,
                    AdminUpdateProfileRequest request,
                    ISender sender,
                    CancellationToken cancellationToken
                ) => await HandleAsync((profileId, request), sender, cancellationToken)
            )
            .WithName("Редактировать профиль (админ)")
            .WithTags("Администрирование")
            .WithSummary("Редактирование профиля пользователя администратором")
            .WithDescription(
                "Обновляет все данные профиля (ФИО, дата рождения, описание, контакты, опыт, образование, навыки) "
                    + "с указанием причины. Пользователь автоматически получает уведомление об изменении."
            )
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status422UnprocessableEntity)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization(Authorization.Policies.Admin);
    }

    public async Task<NoContent> HandleAsync(
        (Guid ProfileId, AdminUpdateProfileRequest Request) input,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var command = new AdminUpdateProfileCommand(
            input.ProfileId,
            input.Request.FirstName,
            input.Request.LastName,
            input.Request.MiddleName,
            input.Request.BirthDate,
            input.Request.Bio,
            input.Request.Contacts,
            input.Request.EmploymentHistories,
            input.Request.Educations,
            input.Request.Skills,
            input.Request.Reason
        );
        await sender.Send(command, cancellationToken);
        return TypedResults.NoContent();
    }
}
