namespace Edvantix.Organizational.Features.Directories.PaymentMethods.Archive;

/// <summary>Эндпоинт архивации способа оплаты.</summary>
public sealed class ArchivePaymentMethodEndpoint : IEndpoint<NoContent, Guid, ISender>
{
    /// <inheritdoc/>
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/directories/payment-methods/{id:guid}/archive",
                async (Guid id, ISender sender, CancellationToken cancellationToken) =>
                    await HandleAsync(id, sender, cancellationToken)
            )
            .WithName("ArchivePaymentMethod")
            .WithTags("Способы оплаты")
            .WithSummary("Архивировать способ оплаты")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    /// <inheritdoc/>
    public async Task<NoContent> HandleAsync(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(new ArchivePaymentMethodCommand(id), cancellationToken);

        return TypedResults.NoContent();
    }
}
