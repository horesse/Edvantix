using Edvantix.Organizational.Domain.AggregatesModel.InvitationAggregate;

namespace Edvantix.Organizational.Infrastructure.Repositories;

/// <summary>
/// Репозиторий приглашений. Использует CrudRepository (не SoftDelete — статусный lifecycle).
/// </summary>
public sealed class InvitationRepository : IInvitationRepository
{
    public IUnitOfWork UnitOfWork { get; }
}
