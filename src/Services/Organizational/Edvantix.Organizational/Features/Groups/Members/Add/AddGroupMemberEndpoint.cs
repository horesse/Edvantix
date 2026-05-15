using Edvantix.Organizational.Domain.AggregatesModel.GroupAggregate;

namespace Edvantix.Organizational.Features.Groups.Members.Add;

public sealed class AddGroupMemberEndpoint
    : IEndpoint<Created<Guid>, AddGroupMemberCommand, ISender, LinkGenerator>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/groups/{groupId:guid}/members",
                async (
                    Guid groupId,
                    AddGroupMemberRequest body,
                    ISender sender,
                    LinkGenerator linker,
                    CancellationToken cancellationToken
                ) =>
                    await HandleAsync(
                        new AddGroupMemberCommand(
                            groupId,
                            body.ProfileId,
                            body.Role,
                            body.JoinedAt
                        ),
                        sender,
                        linker,
                        cancellationToken
                    )
            )
            .WithName("AddGroupMember")
            .WithTags("Участники группы")
            .WithSummary("Добавить участника в группу")
            .Produces<Guid>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status409Conflict)
            .Produces(StatusCodes.Status422UnprocessableEntity)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<Created<Guid>> HandleAsync(
        AddGroupMemberCommand command,
        ISender sender,
        LinkGenerator linker,
        CancellationToken cancellationToken = default
    )
    {
        var id = await sender.Send(command, cancellationToken);
        var location =
            linker.GetPathByName("GetGroupMembers", new { groupId = command.GroupId })
            ?? $"/api/groups/{command.GroupId}/members";

        return TypedResults.Created(location, id);
    }
}

/// <summary>Тело запроса для добавления участника в группу.</summary>
public sealed record AddGroupMemberRequest(
    [property: Description("Идентификатор профиля пользователя")] Guid ProfileId,
    [property: Description("Роль в группе")] GroupMemberRole Role,
    [property: Description("Дата вступления")] DateOnly JoinedAt
);
