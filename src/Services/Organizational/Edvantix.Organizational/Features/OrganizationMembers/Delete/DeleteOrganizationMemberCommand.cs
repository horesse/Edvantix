using Edvantix.Organizational.Domain.AggregatesModel.OrganizationMemberAggregate;

namespace Edvantix.Organizational.Features.OrganizationMembers.Delete;

public sealed record DeleteOrganizationMemberCommand(Guid OrganizationId, Guid Id) : ICommand;

internal sealed class DeleteOrganizationMemberCommandHandler(
    IOrganizationMemberRepository repository
) : ICommandHandler<DeleteOrganizationMemberCommand>
{
    public async ValueTask<Unit> Handle(
        DeleteOrganizationMemberCommand command,
        CancellationToken cancellationToken
    )
    {
        var member = await repository.GetByIdAsync(command.Id, cancellationToken);

        if (member is null || member.OrganizationId != command.OrganizationId)
            throw NotFoundException.For<OrganizationMember>(command.Id);

        member.Delete();

        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
