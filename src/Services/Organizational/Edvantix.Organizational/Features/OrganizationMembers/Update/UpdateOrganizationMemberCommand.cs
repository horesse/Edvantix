using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.OrganizationMemberAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.PermissionAggregate;

namespace Edvantix.Organizational.Features.OrganizationMembers.Update;

[Transactional]
[RequirePermission(OrganizationPermissions.ManageMembers)]
public sealed record UpdateOrganizationMemberCommand(Guid Id, Guid OrganizationMemberRoleId)
    : ICommand;

internal sealed class UpdateOrganizationMemberCommandHandler(
    ITenantContext tenantContext,
    IOrganizationMemberRepository repository
) : ICommandHandler<UpdateOrganizationMemberCommand>
{
    public async ValueTask<Unit> Handle(
        UpdateOrganizationMemberCommand command,
        CancellationToken cancellationToken
    )
    {
        var member = await repository.GetByIdAsync(command.Id, cancellationToken);

        if (member is null || member.OrganizationId != tenantContext.OrganizationId)
            throw NotFoundException.For<OrganizationMember>(command.Id);

        member.ChangeRole(command.OrganizationMemberRoleId);

        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
