using Edvantix.Organizational.Domain.AggregatesModel.OrganizationMemberAggregate;

namespace Edvantix.Organizational.Features.OrganizationMembers.Create;

public sealed record CreateOrganizationMemberCommand(
    Guid OrganizationId,
    Guid ProfileId,
    Guid OrganizationMemberRoleId,
    DateOnly StartDate,
    DateOnly? EndDate = null
) : ICommand<Guid>;

internal sealed class CreateOrganizationMemberCommandHandler(
    IOrganizationMemberRepository repository
) : ICommandHandler<CreateOrganizationMemberCommand, Guid>
{
    public async ValueTask<Guid> Handle(
        CreateOrganizationMemberCommand command,
        CancellationToken cancellationToken
    )
    {
        var member = new OrganizationMember(
            command.OrganizationId,
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
