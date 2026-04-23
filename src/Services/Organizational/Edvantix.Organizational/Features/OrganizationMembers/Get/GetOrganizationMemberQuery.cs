using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.OrganizationMemberAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.PermissionAggregate;
using Edvantix.Organizational.Grpc.Services.Profiles;

namespace Edvantix.Organizational.Features.OrganizationMembers.Get;

[RequirePermission(OrganizationPermissions.Read)]
public sealed record GetOrganizationMemberQuery(Guid Id) : IQuery<OrganizationMemberDto>;

internal sealed class GetOrganizationMemberQueryHandler(
    ITenantContext tenantContext,
    IOrganizationMemberRepository repository,
    IMapper<OrganizationMember, OrganizationMemberDto> mapper,
    IProfileService profileService
) : IQueryHandler<GetOrganizationMemberQuery, OrganizationMemberDto>
{
    public async ValueTask<OrganizationMemberDto> Handle(
        GetOrganizationMemberQuery query,
        CancellationToken cancellationToken
    )
    {
        var member = await repository.GetByIdAsync(query.Id, cancellationToken);

        if (member is null || member.OrganizationId != tenantContext.OrganizationId)
            throw NotFoundException.For<OrganizationMember>(query.Id);

        var profile = await profileService.GetProfileByIdAsync(
            member.ProfileId.ToString(),
            cancellationToken
        );
        Guard.Against.NotFound(profile, member.ProfileId);

        return mapper.Map(member) with
        {
            FullName = profile!.FullName,
            LastActivity = DateTime.UtcNow.AddMinutes(-30), // заменить на данные из модуля аудита
        };
    }
}
