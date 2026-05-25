namespace Edvantix.Organizational.Features.Directories.PaymentMethods.Update;

/// <summary>Эндпоинт обновления способа оплаты.</summary>
public sealed class UpdatePaymentMethodEndpoint
    : IEndpoint<Ok<PaymentMethodDto>, UpdatePaymentMethodCommand, ISender>
{
    /// <inheritdoc/>
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(
                "/directories/payment-methods/{id:guid}",
                async (
                    UpdatePaymentMethodCommand command,
                    ISender sender,
                    CancellationToken cancellationToken
                ) => await HandleAsync(command, sender, cancellationToken)
            )
            .WithName("UpdatePaymentMethod")
            .WithTags("Способы оплаты")
            .WithSummary("Обновить способ оплаты")
            .Produces<PaymentMethodDto>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status401Unauthorized)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    /// <inheritdoc/>
    public async Task<Ok<PaymentMethodDto>> HandleAsync(
        UpdatePaymentMethodCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var dto = await sender.Send(command, cancellationToken);

        return TypedResults.Ok(dto);
    }
}
