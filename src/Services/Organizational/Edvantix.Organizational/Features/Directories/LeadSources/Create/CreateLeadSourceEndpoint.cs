namespace Edvantix.Organizational.Features.Directories.LeadSources.Create;

/// <summary>Эндпоинт создания источника привлечения.</summary>
public sealed class CreateLeadSourceEndpoint
    : IEndpoint<Created<LeadSourceDto>, CreateLeadSourceCommand, ISender>
{
    /// <inheritdoc/>
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/directories/sources",
                async (
                    CreateLeadSourceCommand command,
                    ISender sender,
                    CancellationToken cancellationToken
                ) => await HandleAsync(command, sender, cancellationToken)
            )
            .WithName("CreateLeadSource")
            .WithTags("Источники привлечения")
            .WithSummary("Создать источник привлечения")
            .ProducesPost<LeadSourceDto>()
            .Produces(StatusCodes.Status401Unauthorized)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    /// <inheritdoc/>
    public async Task<Created<LeadSourceDto>> HandleAsync(
        CreateLeadSourceCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var dto = await sender.Send(command, cancellationToken);

        return TypedResults.Created($"/api/v1/directories/sources/{dto.Id}", dto);
    }
}
