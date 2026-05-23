namespace Edvantix.Organizational.Features.Directories.PaymentMethods.Restore;

/// <summary>Эндпоинт восстановления способа оплаты из архива.</summary>
public sealed class RestorePaymentMethodEndpoint : IEndpoint<NoContent, Guid, ISender>
{
    /// <inheritdoc/>
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/directories/payment-methods/{id:guid}/restore",
                async (Guid id, ISender sender, CancellationToken cancellationToken) =>
                    await HandleAsync(id, sender, cancellationToken)
            )
            .WithName("RestorePaymentMethod")
            .WithTags("Способы оплаты")
            .WithSummary("Восстановить способ оплаты из архива")
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
        await sender.Send(new RestorePaymentMethodCommand(id), cancellationToken);

        return TypedResults.NoContent();
    }
}
