using Edvantix.Organizational.Features.LegalFormFeature.Models;

namespace Edvantix.Organizational.Features.LegalFormFeature.Features.GetLegalForms;

public sealed class GetLegalFormsEndpoint
    : IEndpoint<Ok<IReadOnlyList<LegalFormModel>>, GetLegalFormsQuery, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/legal-forms",
                async (ISender sender, CancellationToken ct) =>
                    await HandleAsync(new GetLegalFormsQuery(), sender, ct)
            )
            .WithName("GetLegalForms")
            .WithTags("LegalForms")
            .WithSummary("Получить список организационно-правовых форм")
            .WithDescription("Возвращает справочник организационно-правовых форм.")
            .Produces<IReadOnlyList<LegalFormModel>>(StatusCodes.Status200OK)
            .RequireAuthorization();
    }

    public async Task<Ok<IReadOnlyList<LegalFormModel>>> HandleAsync(
        GetLegalFormsQuery query,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(query, cancellationToken);
        return TypedResults.Ok(result);
    }
}
