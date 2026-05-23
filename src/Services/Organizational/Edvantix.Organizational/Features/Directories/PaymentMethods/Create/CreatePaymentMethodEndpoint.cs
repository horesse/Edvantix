namespace Edvantix.Organizational.Features.Directories.PaymentMethods.Create;

/// <summary>Эндпоинт создания способа оплаты.</summary>
public sealed class CreatePaymentMethodEndpoint
    : IEndpoint<Created<PaymentMethodDto>, CreatePaymentMethodCommand, ISender>
{
    /// <inheritdoc/>
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/directories/payment-methods",
                async (
                    CreatePaymentMethodCommand command,
                    ISender sender,
                    CancellationToken cancellationToken
                ) => await HandleAsync(command, sender, cancellationToken)
            )
            .WithName("CreatePaymentMethod")
            .WithTags("Способы оплаты")
            .WithSummary("Создать способ оплаты")
            .ProducesPost<PaymentMethodDto>()
            .Produces(StatusCodes.Status401Unauthorized)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    /// <inheritdoc/>
    public async Task<Created<PaymentMethodDto>> HandleAsync(
        CreatePaymentMethodCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var dto = await sender.Send(command, cancellationToken);

        return TypedResults.Created($"/api/v1/directories/payment-methods/{dto.Id}", dto);
    }
}
