using Edvantix.SharedKernel.Helpers;
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

        builder.Property(p => p.Id).HasDefaultValueSql(UniqueIdentifierHelper.NewUuidV7);
    }

    public static void ConfigureSoftDeletable<TEntity>(this EntityTypeBuilder<TEntity> builder)
        where TEntity : Entity, ISoftDelete
    {
        builder.UseDefaultConfiguration();

        builder.Property(bd => bd.IsDeleted).HasComment("Признак удаленной записи");

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
