using Edvantix.Organizational.Features.OrganizationFeature.Models;
using Edvantix.Organizational.Grpc.Services;

namespace Edvantix.Organizational.Features.OrganizationFeature.Features.GetMyOrganizations;

public sealed record GetMyOrganizationsQuery : IRequest<IEnumerable<OrganizationSummaryModel>>;

public sealed class GetMyOrganizationsQueryHandler(IServiceProvider provider)
    : IRequestHandler<GetMyOrganizationsQuery, IEnumerable<OrganizationSummaryModel>>
{
    public async ValueTask<IEnumerable<OrganizationSummaryModel>> Handle(
        GetMyOrganizationsQuery request,
        CancellationToken cancellationToken
    )
    {
        var profileId = await provider.GetProfileId(cancellationToken);

        // Получаем все членства профиля (без фильтра по организации)
        var memberSpec = new OrganizationMemberSpecification(profileId, (ulong?)null);
        var memberRepo = provider.GetRequiredService<IOrganizationMemberRepository>();
        var members = await memberRepo.ListAsync(memberSpec, cancellationToken);

        if (members.Count == 0)
            return [];

        var orgRepo = provider.GetRequiredService<IOrganizationRepository>();
        var result = new List<OrganizationSummaryModel>(members.Count);

        foreach (var member in members)
        {
            var org = await orgRepo.FindByIdAsync(member.OrganizationId, cancellationToken);
            if (org is null)
                continue;

            result.Add(
                new OrganizationSummaryModel(
                    org.Id,
                    org.Name,
                    org.ShortName,
                    org.Description,
                    member.Role.ToString()
                )
            );
        }

        return result;
    }
}
