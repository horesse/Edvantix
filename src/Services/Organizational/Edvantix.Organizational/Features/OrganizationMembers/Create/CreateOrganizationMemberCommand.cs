using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.OrganizationMemberAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.PermissionAggregate;

namespace Edvantix.Organizational.Features.OrganizationMembers.Create;

[Transactional]
[RequirePermission(OrganizationPermissions.ManageMembers)]
public sealed record CreateOrganizationMemberCommand(
    Guid ProfileId,
    Guid OrganizationMemberRoleId,
    DateOnly StartDate,
    DateOnly? EndDate = null
) : ICommand<Guid>;

internal sealed class CreateOrganizationMemberCommandHandler(
    ITenantContext tenantContext,
    IOrganizationMemberRepository repository
) : ICommandHandler<CreateOrganizationMemberCommand, Guid>
{
    public async ValueTask<Guid> Handle(
        CreateOrganizationMemberCommand command,
        CancellationToken cancellationToken
    )
    {
        var member = new OrganizationMember(
            tenantContext.OrganizationId,
            command.ProfileId,
            command.OrganizationMemberRoleId,
            command.StartDate,
            command.EndDate
        );

        await repository.AddAsync(member, cancellationToken);
        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return member.Id;
    }
}
