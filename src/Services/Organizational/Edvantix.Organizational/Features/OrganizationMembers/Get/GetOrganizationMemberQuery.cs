using Edvantix.Organizational.Domain.AggregatesModel.OrganizationMemberAggregate;

namespace Edvantix.Organizational.Features.OrganizationMembers.Get;

public sealed record GetOrganizationMemberQuery(Guid OrganizationId, Guid Id)
    : IQuery<OrganizationMemberDto>;

internal sealed class GetOrganizationMemberQueryHandler(
    IOrganizationMemberRepository repository,
    IMapper<OrganizationMember, OrganizationMemberDto> mapper
) : IQueryHandler<GetOrganizationMemberQuery, OrganizationMemberDto>
{
    public async ValueTask<OrganizationMemberDto> Handle(
        GetOrganizationMemberQuery query,
        CancellationToken cancellationToken
    )
    {
        var member = await repository.GetByIdAsync(query.Id, cancellationToken);

        if (member is null || member.OrganizationId != query.OrganizationId)
            throw NotFoundException.For<OrganizationMember>(query.Id);

        return mapper.Map(member);
    }
}
