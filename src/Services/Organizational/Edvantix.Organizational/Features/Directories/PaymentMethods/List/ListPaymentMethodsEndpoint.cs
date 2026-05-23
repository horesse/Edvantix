namespace Edvantix.Organizational.Features.Directories.PaymentMethods.List;

/// <summary>Эндпоинт постраничного списка способов оплаты.</summary>
public sealed class ListPaymentMethodsEndpoint
    : IEndpoint<Ok<PagedResult<PaymentMethodListItemDto>>, ListPaymentMethodsQuery, ISender>
{
    /// <inheritdoc/>
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/directories/payment-methods",
                async (
                    [AsParameters] ListPaymentMethodsQuery query,
                    ISender sender,
                    CancellationToken cancellationToken
                ) => await HandleAsync(query, sender, cancellationToken)
            )
            .WithName("ListPaymentMethods")
            .WithTags("Способы оплаты")
            .WithSummary("Получить список способов оплаты организации")
            .ProducesGet<PagedResult<PaymentMethodListItemDto>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    /// <inheritdoc/>
    public async Task<Ok<PagedResult<PaymentMethodListItemDto>>> HandleAsync(
        ListPaymentMethodsQuery query,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(query, cancellationToken);

        return TypedResults.Ok(result);
    }
}
