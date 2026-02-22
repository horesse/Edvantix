namespace Edvantix.Blog.Features.TagFeature.UpdateTag;

/// <summary>
/// Запрос на обновление тега от клиента.
/// </summary>
public sealed record UpdateTagRequest(string Name, string Slug);

/// <summary>
/// Административный эндпоинт для обновления тега блога.
/// </summary>
public sealed class UpdateTagEndpoint : IEndpoint<NoContent, UpdateTagCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(
                "/admin/tags/{tagId:long}",
                async (
                    ulong tagId,
                    UpdateTagRequest request,
                    ISender sender,
                    CancellationToken ct
                ) =>
                    await HandleAsync(
                        new UpdateTagCommand(tagId, request.Name, request.Slug),
                        sender,
                        ct
                    )
            )
            .WithName("UpdateTag")
            .WithTags("Admin.Tags")
            .WithSummary("Обновить тег")
            .WithDescription("Обновляет данные тега блога. Доступно только администраторам.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status403Forbidden)
            .RequireAuthorization(Authorization.Policies.Admin);
    }

    public async Task<NoContent> HandleAsync(
        UpdateTagCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(command, cancellationToken);
        return TypedResults.NoContent();
    }
}
