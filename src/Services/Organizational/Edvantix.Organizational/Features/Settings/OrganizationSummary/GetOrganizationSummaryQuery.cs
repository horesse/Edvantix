using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.OrganizationAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.OrganizationMemberAggregate;
using Edvantix.Organizational.Domain.Permissions;
using Edvantix.Organizational.Features.Organizations;
using Edvantix.Organizational.Grpc.Services.Profiles;
using ZiggyCreatures.Caching.Fusion;

namespace Edvantix.Organizational.Features.Settings.OrganizationSummary;

[RequirePermission(OrganizationPermissions.View)]
public sealed record GetOrganizationSummaryQuery : IQuery<OrganizationSummaryDto>;

internal sealed class GetOrganizationSummaryQueryHandler(
    ITenantContext tenantContext,
    IOrganizationRepository repository,
    IOrganizationMemberRepository memberRepository,
    IProfileService profileService,
    IFusionCache cache
) : IQueryHandler<GetOrganizationSummaryQuery, OrganizationSummaryDto>
{
    private const string DeletedUserDisplayName = "Удалённый пользователь";

    public async ValueTask<OrganizationSummaryDto> Handle(
        GetOrganizationSummaryQuery query,
        CancellationToken cancellationToken
    )
    {
        var orgId = tenantContext.OrganizationId;
        var cacheKey = $"org:{orgId}:summary";

        return await cache.GetOrSetAsync(
            cacheKey,
            async ct =>
            {
                var organization = await repository.GetByIdAsync(orgId, ct);
                Guard.Against.NotFound(organization, orgId);

                var countSpec = new OrganizationMemberSpecification(
                    orgId,
                    status: OrganizationStatus.Active
                );
                var membersCount = await memberRepository.CountAsync(countSpec, ct);

                var displayName = await ResolveLastModifiedByAsync(organization.LastModifiedBy, ct);

                var primaryContact = organization.Contacts.FirstOrDefault(c => c.IsPrimary);

                return new OrganizationSummaryDto(
                    organization.Id,
                    organization.FullLegalName,
                    organization.ShortName,
                    organization.OrganizationType,
                    organization.Status,
                    organization.IsLegalEntity,
                    membersCount,
                    primaryContact is null
                        ? null
                        : new ContactDto(
                            primaryContact.Id,
                            primaryContact.Value,
                            primaryContact.Description,
                            primaryContact.ContactType,
                            primaryContact.IsPrimary
                        ),
                    new OrganizationSummaryDto.LastModifiedInfo(
                        organization.LastModifiedAt,
                        displayName
                    )
                );
            },
            options => options.SetDuration(TimeSpan.FromSeconds(60)),
            token: cancellationToken
        );
    }

    private async Task<string?> ResolveLastModifiedByAsync(
        Guid? lastModifiedBy,
        CancellationToken cancellationToken
    )
    {
        if (lastModifiedBy is null)
            return null;

        var profile = await profileService.GetProfileByIdAsync(
            lastModifiedBy.Value.ToString("D"),
            cancellationToken
        );

        return profile?.FullName ?? DeletedUserDisplayName;
    }
}
