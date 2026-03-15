using Edvantix.Persona.Domain.AggregatesModel.SkillAggregate;

namespace Edvantix.Persona.Infrastructure;

public sealed class PersonaDbContext(DbContextOptions<PersonaDbContext> options)
    : DbContext(options),
        IUnitOfWork
{
    public DbSet<Profile> Profiles => Set<Profile>();
    public DbSet<Skill> Skills => Set<Skill>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PersonaDbContext).Assembly);
    }

    public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        await SaveChangesAsync(cancellationToken);
        return true;
    }
}
