using Edvantix.Organizational.Domain.Enums;

namespace Edvantix.Organizational.Domain.AggregatesModel.InvitationAggregate;

/// <summary>Находит единственное активное (Pending) приглашение по SHA-256 хэшу токена.</summary>
public sealed class InvitationByTokenHashSpecification : Specification<Invitation>
{
    public InvitationByTokenHashSpecification(string tokenHash)
    {
        Query
            .AsTracking()
            .Where(i =>
                i.TokenHash == tokenHash && i.Status == InvitationStatus.Pending && !i.IsDeleted
            );
    }
}
