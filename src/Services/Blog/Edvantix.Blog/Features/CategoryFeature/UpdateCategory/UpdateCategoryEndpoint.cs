namespace Edvantix.Blog.Features.CategoryFeature.UpdateCategory;

/// <summary>
/// Запрос на обновление категории от клиента.
/// </summary>
public sealed record UpdateCategoryRequest(string Name, string Slug, string? Description);

/// <summary>
/// Административный эндпоинт для обновления категории блога.
/// </summary>
public sealed class UpdateCategoryEndpoint : IEndpoint<NoContent, UpdateCategoryCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(
                "/admin/categories/{categoryId:guid}",
                async (
                    Guid categoryId,
                    UpdateCategoryRequest request,
                    ISender sender,
                    CancellationToken ct
                ) =>
                    await HandleAsync(
                        new UpdateCategoryCommand(
                            categoryId,
                            request.Name,
                            request.Slug,
                            request.Description
                        ),
                        sender,
                        ct
                    )
            )
            .WithName("UpdateCategory")
            .WithTags("Admin.Categories")
            .WithSummary("Обновить категорию")
            .WithDescription("Обновляет данные категории блога. Доступно только администраторам.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status403Forbidden)
            .RequireAuthorization(Authorization.Policies.Admin);
    }

    public async Task<NoContent> HandleAsync(
        UpdateCategoryCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(command, cancellationToken);
        return TypedResults.NoContent();
    }
}
