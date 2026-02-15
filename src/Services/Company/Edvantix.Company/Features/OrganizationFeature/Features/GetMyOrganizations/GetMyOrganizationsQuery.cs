using Edvantix.Company.Domain.AggregatesModel.OrganizationAggregate;
using Edvantix.Company.Domain.AggregatesModel.OrganizationMemberAggregate;
using Edvantix.Company.Domain.AggregatesModel.OrganizationMemberAggregate.Specifications;
using Edvantix.Company.Features.OrganizationFeature.Models;
using Edvantix.Company.Grpc.Services;
using MediatR;

namespace Edvantix.Company.Features.OrganizationFeature.Features.GetMyOrganizations;

public sealed record GetMyOrganizationsQuery : IRequest<IEnumerable<OrganizationSummaryModel>>;

public sealed class GetMyOrganizationsQueryHandler(IServiceProvider provider)
    : IRequestHandler<GetMyOrganizationsQuery, IEnumerable<OrganizationSummaryModel>>
{
    public async Task<IEnumerable<OrganizationSummaryModel>> Handle(
        GetMyOrganizationsQuery request,
        CancellationToken cancellationToken
    )
    {
        var profileId = await provider.GetProfileId(cancellationToken);

        var spec = new OrganizationMemberByProfileSpecification(profileId);

        using var memberRepo = provider.GetRequiredService<IOrganizationMemberRepository>();

        var members = await memberRepo.GetByExpressionAsync(spec, cancellationToken);

        if (members.Count == 0)
            return [];

        using var orgRepo = provider.GetRequiredService<IOrganizationRepository>();

        var result = new List<OrganizationSummaryModel>();

        foreach (var member in members)
        {
            var org = await orgRepo.GetByIdAsync(member.OrganizationId, cancellationToken);
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
