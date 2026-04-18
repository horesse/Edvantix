using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.OrganizationMemberAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.PermissionAggregate;

namespace Edvantix.Organizational.Features.OrganizationMembers.Delete;

[Transactional]
[RequirePermission(OrganizationPermissions.ManageMembers)]
public sealed record DeleteOrganizationMemberCommand(Guid Id) : ICommand;

internal sealed class DeleteOrganizationMemberCommandHandler(
    ITenantContext tenantContext,
    IOrganizationMemberRepository repository
) : ICommandHandler<DeleteOrganizationMemberCommand>
{
    public async ValueTask<Unit> Handle(
        DeleteOrganizationMemberCommand command,
        CancellationToken cancellationToken
    )
    {
        var member = await repository.GetByIdAsync(command.Id, cancellationToken);

        if (member is null || member.OrganizationId != tenantContext.OrganizationId)
            throw NotFoundException.For<OrganizationMember>(command.Id);

        member.Delete();

        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
