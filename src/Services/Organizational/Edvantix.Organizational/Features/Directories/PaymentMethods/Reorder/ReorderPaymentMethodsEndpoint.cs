namespace Edvantix.Organizational.Features.Directories.PaymentMethods.Reorder;

/// <summary>PATCH /api/v1/directories/payment-methods/reorder — переупорядочить способы оплаты.</summary>
public sealed class ReorderPaymentMethodsEndpoint
    : IEndpoint<NoContent, ReorderPaymentMethodsCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch(
                "/directories/payment-methods/reorder",
                async (
                    ReorderPaymentMethodsCommand command,
                    ISender sender,
                    CancellationToken cancellationToken
                ) => await HandleAsync(command, sender, cancellationToken)
            )
            .WithName("ReorderPaymentMethods")
            .WithTags("Справочник: Способы оплаты")
            .WithSummary("Изменить порядок способов оплаты")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<NoContent> HandleAsync(
        ReorderPaymentMethodsCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(command, cancellationToken);

        return TypedResults.NoContent();
    }
}
