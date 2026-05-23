namespace Edvantix.Organizational.Features.Directories.LeadSources.Update;

/// <summary>Эндпоинт обновления источника привлечения.</summary>
public sealed class UpdateLeadSourceEndpoint
    : IEndpoint<Ok<LeadSourceDto>, UpdateLeadSourceCommand, ISender>
{
    /// <inheritdoc/>
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(
                "/directories/sources/{id:guid}",
                async (
                    UpdateLeadSourceCommand command,
                    ISender sender,
                    CancellationToken cancellationToken
                ) => await HandleAsync(command, sender, cancellationToken)
            )
            .WithName("UpdateLeadSource")
            .WithTags("Источники привлечения")
            .WithSummary("Обновить источник привлечения")
            .Produces<LeadSourceDto>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status401Unauthorized)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    /// <inheritdoc/>
    public async Task<Ok<LeadSourceDto>> HandleAsync(
        UpdateLeadSourceCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var dto = await sender.Send(command, cancellationToken);

        return TypedResults.Ok(dto);
    }
}
