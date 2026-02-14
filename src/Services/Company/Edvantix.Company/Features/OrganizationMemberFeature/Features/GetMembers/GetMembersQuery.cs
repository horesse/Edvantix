using Edvantix.Company.Domain.AggregatesModel.OrganizationMemberAggregate;
using Edvantix.Company.Domain.AggregatesModel.OrganizationMemberAggregate.Specifications;
using Edvantix.Company.Features.OrganizationMemberFeature.Models;
using Edvantix.Company.Services;
using MediatR;

namespace Edvantix.Company.Features.OrganizationMemberFeature.Features.GetMembers;

public sealed record GetMembersQuery(long OrganizationId)
    : IRequest<IEnumerable<OrganizationMemberModel>>;

public sealed class GetMembersQueryHandler(IServiceProvider provider)
    : IRequestHandler<GetMembersQuery, IEnumerable<OrganizationMemberModel>>
{
    public async Task<IEnumerable<OrganizationMemberModel>> Handle(
        GetMembersQuery request,
        CancellationToken cancellationToken
    )
    {
        var authService = provider.GetRequiredService<IOrganizationAuthorizationService>();
        await authService.GetCurrentMemberAsync(request.OrganizationId, cancellationToken);

        var spec = new OrganizationMemberSpecification { OrganizationId = request.OrganizationId };

        using var memberRepo = provider.GetRequiredService<IOrganizationMemberRepository>();

        var members = await memberRepo.GetByExpressionAsync(spec, cancellationToken);

        // TODO: Fetch user profile data from Profile service via gRPC
        return members.Select(m => new OrganizationMemberModel
        {
            Id = m.Id,
            OrganizationId = m.OrganizationId,
            ProfileId = m.ProfileId,
            Role = m.Role,
            JoinedAt = m.JoinedAt,
        });
    }
}
