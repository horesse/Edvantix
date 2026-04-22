using Edvantix.Organizational.Domain.AggregatesModel.OrganizationMemberAggregate;
using Edvantix.Organizational.Domain.Events;

namespace Edvantix.Organizational.Domain.EventHandlers;

/// <summary>
/// При принятии приглашения создаёт участника организации с ролью, указанной в приглашении.
/// Выполняется в той же транзакционной области, что и смена статуса приглашения.
/// </summary>
internal sealed class InvitationAcceptedDomainEventHandler(
    IOrganizationMemberRepository memberRepository
) : INotificationHandler<InvitationAcceptedDomainEvent>
{
    public async ValueTask Handle(
        InvitationAcceptedDomainEvent notification,
        CancellationToken cancellationToken
    )
    {
        var member = new OrganizationMember(
            notification.OrganizationId,
            notification.AcceptedByProfileId,
            notification.RoleId,
            DateOnly.FromDateTime(DateTime.UtcNow)
        );

        await memberRepository.AddAsync(member, cancellationToken);
        await memberRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}
