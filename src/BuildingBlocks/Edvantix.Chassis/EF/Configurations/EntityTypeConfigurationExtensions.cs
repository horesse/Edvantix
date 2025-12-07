using Edvantix.SharedKernel.SeedWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edvantix.Chassis.EF.Configurations;

public static class EntityTypeConfigurationExtensions
{
    public static void Configure<TEntity, TIdentity>(
        this EntityTypeBuilder<TEntity> builder,
        string scheme,
        string table
    )
        where TIdentity : struct
        where TEntity : Entity<TIdentity>
    {
        builder.ToTable(table, scheme);

        builder.HasKey(bd => bd.Id);

        builder.Property(bd => bd.Id).HasComment("Идентификатор");
    }

    public static void ConfigureSoftDeletable<TEntity, TIdentity>(
        this EntityTypeBuilder<TEntity> builder,
        string scheme,
        string table
    )
        where TIdentity : struct
        where TEntity : Entity<TIdentity>, ISoftDelete
    {
        builder.ToTable(table, scheme);

        builder.HasKey(bd => bd.Id);

        builder.Property(bd => bd.Id).HasComment("Идентификатор");

        builder.Property(bd => bd.IsDeleted).HasComment("Признак удаленной записи");
    }
}
