namespace Edvantix.Organizational.Features.Directories.PaymentMethods.GetById;

/// <summary>Эндпоинт получения способа оплаты по идентификатору.</summary>
public sealed class GetPaymentMethodByIdEndpoint
    : IEndpoint<Results<Ok<PaymentMethodDto>, NotFound>, Guid, ISender>
{
    /// <inheritdoc/>
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/directories/payment-methods/{id:guid}",
                async (Guid id, ISender sender, CancellationToken cancellationToken) =>
                    await HandleAsync(id, sender, cancellationToken)
            )
            .WithName("GetPaymentMethodById")
            .WithTags("Способы оплаты")
            .WithSummary("Получить способ оплаты по идентификатору")
            .ProducesGet<PaymentMethodDto>(hasNotFound: true)
            .Produces(StatusCodes.Status401Unauthorized)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    /// <inheritdoc/>
    public async Task<Results<Ok<PaymentMethodDto>, NotFound>> HandleAsync(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var dto = await sender.Send(new GetPaymentMethodByIdQuery(id), cancellationToken);

            return TypedResults.Ok(dto);
        }
        catch (NotFoundException)
        {
            return TypedResults.NotFound();
        }
    }
}
