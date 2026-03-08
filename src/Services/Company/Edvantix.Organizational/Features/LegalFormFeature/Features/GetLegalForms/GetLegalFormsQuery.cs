using Edvantix.Organizational.Domain.AggregatesModel.LegalFormAggregate;
using Edvantix.Organizational.Features.LegalFormFeature.Models;

namespace Edvantix.Organizational.Features.LegalFormFeature.Features.GetLegalForms;

public sealed record GetLegalFormsQuery : IQuery<IReadOnlyList<LegalFormModel>>;

public sealed class GetLegalFormsQueryHandler(IServiceProvider provider)
    : IQueryHandler<GetLegalFormsQuery, IReadOnlyList<LegalFormModel>>
{
    public async ValueTask<IReadOnlyList<LegalFormModel>> Handle(
        GetLegalFormsQuery request,
        CancellationToken cancellationToken
    )
    {
        var repo = provider.GetRequiredService<ILegalFormRepository>();
        var legalForms = await repo.ListAllAsync(cancellationToken);

        var mapper = provider.GetRequiredService<IMapper<LegalForm, LegalFormModel>>();

        return mapper.Map(legalForms);
    }
}
