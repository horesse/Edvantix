namespace Edvantix.Organizational.Features.Directories.LeadSources.GetById;

/// <summary>Эндпоинт получения источника привлечения по идентификатору.</summary>
public sealed class GetLeadSourceByIdEndpoint
    : IEndpoint<Results<Ok<LeadSourceDto>, NotFound>, Guid, ISender>
{
    /// <inheritdoc/>
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/directories/sources/{id:guid}",
                async (Guid id, ISender sender, CancellationToken cancellationToken) =>
                    await HandleAsync(id, sender, cancellationToken)
            )
            .WithName("GetLeadSourceById")
            .WithTags("Источники привлечения")
            .WithSummary("Получить источник привлечения по идентификатору")
            .ProducesGet<LeadSourceDto>(hasNotFound: true)
            .Produces(StatusCodes.Status401Unauthorized)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    /// <inheritdoc/>
    public async Task<Results<Ok<LeadSourceDto>, NotFound>> HandleAsync(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var dto = await sender.Send(new GetLeadSourceByIdQuery(id), cancellationToken);

            return TypedResults.Ok(dto);
        }
        catch (NotFoundException)
        {
            return TypedResults.NotFound();
        }
    }
}
