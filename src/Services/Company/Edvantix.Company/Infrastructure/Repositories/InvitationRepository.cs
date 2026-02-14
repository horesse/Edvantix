using Edvantix.Chassis.Repository.Crud;
using Edvantix.Company.Domain.AggregatesModel.InvitationAggregate;

namespace Edvantix.Company.Infrastructure.Repositories;

/// <summary>
/// Репозиторий приглашений. Использует CrudRepository (не SoftDelete — статусный lifecycle).
/// </summary>
public sealed class InvitationRepository(IServiceProvider provider)
    : CrudRepository<OrganizationContext, Invitation, Guid>(provider),
        IInvitationRepository;
