using Edvantix.Chassis.Specification.Evaluators;
using Edvantix.Organizational.Domain.AggregatesModel.InvitationAggregate;

namespace Edvantix.Organizational.Infrastructure.Repositories;

internal sealed class InvitationRepository(OrganizationalDbContext context) : IInvitationRepository
{
    public IUnitOfWork UnitOfWork => context;
    private static SpecificationEvaluator Specification => SpecificationEvaluator.Instance;

    public async Task<Invitation?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default
    ) =>
        await context
            .Invitations.AsTracking()
            .FirstOrDefaultAsync(i => i.Id == id && !i.IsDeleted, cancellationToken);

    public async Task<Invitation?> GetByTokenHashAsync(
        string tokenHash,
        CancellationToken cancellationToken = default
    ) =>
        await Specification
            .GetQuery(
                context.Invitations.AsQueryable(),
                new InvitationByTokenHashSpecification(tokenHash)
            )
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<IReadOnlyCollection<Invitation>> ListAsync(
        ISpecification<Invitation> specification,
        CancellationToken cancellationToken = default
    ) =>
        await Specification
            .GetQuery(context.Invitations.AsQueryable(), specification)
            .ToListAsync(cancellationToken);

    public async Task<int> CountAsync(
        ISpecification<Invitation> specification,
        CancellationToken cancellationToken = default
    ) =>
        await Specification
            .GetQuery(context.Invitations.AsQueryable(), specification)
            .CountAsync(cancellationToken);

    public async Task AddAsync(
        Invitation invitation,
        CancellationToken cancellationToken = default
    ) => await context.Invitations.AddAsync(invitation, cancellationToken);
}
