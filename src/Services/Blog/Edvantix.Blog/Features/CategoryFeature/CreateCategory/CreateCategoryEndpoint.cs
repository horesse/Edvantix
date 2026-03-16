namespace Edvantix.Blog.Features.CategoryFeature.CreateCategory;

/// <summary>
/// Запрос на создание категории от клиента.
/// </summary>
public sealed record CreateCategoryRequest(string Name, string Slug, string? Description);

/// <summary>
/// Административный эндпоинт для создания категории блога.
/// </summary>
public sealed class CreateCategoryEndpoint
    : IEndpoint<Created<Guid>, CreateCategoryCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/admin/categories",
                async (
                    CreateCategoryRequest request,
                    ISender sender,
                    CancellationToken cancellationToken
                ) =>
                    await HandleAsync(
                        new CreateCategoryCommand(request.Name, request.Slug, request.Description),
                        sender,
                        cancellationToken
                    )
            )
            .WithName("CreateCategory")
            .WithTags("Admin.Categories")
            .WithSummary("Создать категорию")
            .WithDescription("Создаёт новую категорию блога. Доступно только администраторам.")
            .Produces<Guid>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status403Forbidden)
            .RequireAuthorization(Authorization.Policies.Admin);
    }

    public async Task<Created<Guid>> HandleAsync(
        CreateCategoryCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var id = await sender.Send(command, cancellationToken);
        return TypedResults.Created($"/api/v1/categories/{id}", id);
    }
}
