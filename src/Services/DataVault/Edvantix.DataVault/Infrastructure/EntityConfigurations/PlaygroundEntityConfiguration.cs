using Edvantix.Chassis.EF.Configurations;
using Edvantix.Constants.Core;
using Edvantix.DataVault.Domain.AggregatesModel.PlaygroundEntityAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edvantix.DataVault.Infrastructure.EntityConfigurations;

public sealed class PlaygroundEntityConfiguration : IEntityTypeConfiguration<PlaygroundEntity>
{
    public void Configure(EntityTypeBuilder<PlaygroundEntity> builder)
    {
        builder.ConfigureSoftDeletable<PlaygroundEntity, long>();

        builder
            .Property(p => p.Name)
            .HasMaxLength(DataSchemaLength.Large)
            .IsRequired()
            .HasComment("Наименование");

        builder.Property(p => p.Value).IsRequired().HasComment("Значение");
    }
}
