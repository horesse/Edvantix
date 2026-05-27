using Edvantix.Organizational.Features.Directories;

namespace Edvantix.Organizational.Features.Directories.PaymentMethods.Reorder;

/// <summary>PATCH /api/v1/directories/payment-methods/reorder — переупорядочить способы оплаты.</summary>
public sealed class ReorderPaymentMethodsEndpoint : IEndpoint<NoContent, ReorderRequest, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch(
                "/directories/payment-methods/reorder",
                async (ReorderRequest request, ISender sender, CancellationToken cancellationToken) =>
                    await HandleAsync(request, sender, cancellationToken)
            )
            .WithName("ReorderPaymentMethods")
            .WithTags("Способы оплаты")
            .WithSummary("Изменить порядок способов оплаты")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<NoContent> HandleAsync(
        ReorderRequest request,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(new ReorderPaymentMethodsCommand(request.OrderedIds), cancellationToken);

        return TypedResults.NoContent();
    }
}
