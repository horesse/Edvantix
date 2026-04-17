using Edvantix.Organizational.Domain.AggregatesModel.OrganizationMemberAggregate;

namespace Edvantix.Organizational.Features.OrganizationMembers.Update;

public sealed record UpdateOrganizationMemberCommand(
    Guid OrganizationId,
    Guid Id,
    Guid OrganizationMemberRoleId
) : ICommand;

internal sealed class UpdateOrganizationMemberCommandHandler(
    IOrganizationMemberRepository repository
) : ICommandHandler<UpdateOrganizationMemberCommand>
{
    public async ValueTask<Unit> Handle(
        UpdateOrganizationMemberCommand command,
        CancellationToken cancellationToken
    )
    {
        var member = await repository.GetByIdAsync(command.Id, cancellationToken);

        if (member is null || member.OrganizationId != command.OrganizationId)
            throw NotFoundException.For<OrganizationMember>(command.Id);

        member.ChangeRole(command.OrganizationMemberRoleId);

        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
