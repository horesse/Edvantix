namespace Edvantix.Organizational.Domain.AggregatesModel.InvitationAggregate;

/// <summary>
/// Репозиторий для работы с приглашениями в организацию.
/// </summary>
public interface IInvitationRepository : IRepository<Invitation>
{
    Task<Invitation?> FindAsync(ISpecification<Invitation> spec, CancellationToken ct = default);
    Task<IReadOnlyList<Invitation>> ListAsync(
        ISpecification<Invitation> spec,
        CancellationToken ct = default
    );
    Task<Invitation?> FindByIdAsync(Guid id, CancellationToken ct = default);
    Task<Invitation> AddAsync(Invitation entity, CancellationToken ct = default);
}
