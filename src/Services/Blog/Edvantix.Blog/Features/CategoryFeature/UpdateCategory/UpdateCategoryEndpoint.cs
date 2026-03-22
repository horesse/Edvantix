namespace Edvantix.Blog.Features.CategoryFeature.UpdateCategory;

public sealed record UpdateCategoryRequest(string Name, string Slug, string? Description);

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
                    CancellationToken cancellationToken
                ) =>
                    await HandleAsync(
                        new UpdateCategoryCommand(
                            categoryId,
                            request.Name,
                            request.Slug,
                            request.Description
                        ),
                        sender,
                        cancellationToken
                    )
            )
            .WithName("Обновить категорию")
            .WithTags("Администрирование")
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
