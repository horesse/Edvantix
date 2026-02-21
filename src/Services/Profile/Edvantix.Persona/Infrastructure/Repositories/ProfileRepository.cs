using Edvantix.Chassis.Specification.Evaluators;

namespace Edvantix.Persona.Infrastructure.Repositories;

public sealed class ProfileRepository(PersonaDbContext context) : IProfileRepository
{
    private readonly PersonaDbContext _context =
        context ?? throw new ArgumentNullException(nameof(context));

    private static SpecificationEvaluator Specification => SpecificationEvaluator.Instance;

    public IUnitOfWork UnitOfWork => _context;
}
