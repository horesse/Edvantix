using Edvantix.SharedKernel.SeedWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edvantix.Chassis.EF.Configurations;

public static class EntityTypeConfigurationExtensions
{
    public static void Configure<TEntity, TIdentity>(this EntityTypeBuilder<TEntity> builder)
        where TIdentity : struct
        where TEntity : Entity<TIdentity>
    {
        builder.HasKey(bd => bd.Id);

        builder.Property(bd => bd.Id).HasComment("Идентификатор");
    }

    public static void ConfigureSoftDeletable<TEntity, TIdentity>(
        this EntityTypeBuilder<TEntity> builder
    )
        where TIdentity : struct
        where TEntity : Entity<TIdentity>, ISoftDelete
    {
        builder.HasKey(bd => bd.Id);

        builder.Property(bd => bd.Id).HasComment("Идентификатор");

        builder.Property(bd => bd.IsDeleted).HasComment("Признак удаленной записи");
        
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
