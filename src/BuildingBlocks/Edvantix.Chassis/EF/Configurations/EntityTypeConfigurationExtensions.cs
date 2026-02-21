using Edvantix.SharedKernel.SeedWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edvantix.Chassis.EF.Configurations;

public static class EntityTypeConfigurationExtensions
{
    public static void UseDefaultConfiguration<TEntity>(this EntityTypeBuilder<TEntity> builder)
        where TEntity : Entity
    {
        builder.HasKey(bd => bd.Id);

        builder.Property(bd => bd.Id).HasComment("Идентификатор");
    }

    public static void ConfigureSoftDeletable<TEntity>(this EntityTypeBuilder<TEntity> builder)
        where TEntity : Entity, ISoftDelete
    {
        builder.HasKey(bd => bd.Id);

        builder.Property(bd => bd.Id).HasComment("Идентификатор");

        builder.Property(bd => bd.IsDeleted).HasComment("Признак удаленной записи");

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
