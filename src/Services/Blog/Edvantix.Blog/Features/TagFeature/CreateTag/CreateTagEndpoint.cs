namespace Edvantix.Blog.Features.TagFeature.CreateTag;

/// <summary>
/// Запрос на создание тега от клиента.
/// </summary>
public sealed record CreateTagRequest(string Name, string Slug);

/// <summary>
/// Административный эндпоинт для создания тега блога.
/// </summary>
public sealed class CreateTagEndpoint : IEndpoint<Created<Guid>, CreateTagCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/admin/tags",
                async (CreateTagRequest request, ISender sender, CancellationToken ct) =>
                    await HandleAsync(new CreateTagCommand(request.Name, request.Slug), sender, ct)
            )
            .WithName("CreateTag")
            .WithTags("Admin.Tags")
            .WithSummary("Создать тег")
            .WithDescription("Создаёт новый тег блога. Доступно только администраторам.")
            .Produces<Guid>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status403Forbidden)
            .RequireAuthorization(Authorization.Policies.Admin);
    }

    public async Task<Created<Guid>> HandleAsync(
        CreateTagCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var id = await sender.Send(command, cancellationToken);
        return TypedResults.Created($"/api/v1/tags/{id}", id);
    }
}
