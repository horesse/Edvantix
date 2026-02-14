using Edvantix.Chassis.Repository.Crud;

namespace Edvantix.Company.Domain.AggregatesModel.InvitationAggregate;

/// <summary>
/// Репозиторий для работы с приглашениями в организацию.
/// </summary>
public interface IInvitationRepository : ICrudRepository<Invitation, Guid>;
