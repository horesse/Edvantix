using Edvantix.Chassis.EF.Configurations;
using Edvantix.Constants.Core;
using Edvantix.Constants.Other;
using Edvantix.EntityHub.Domain.AggregatesModel.EntityGroupAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edvantix.EntityHub.Infrastructure.EntityConfigurations;

public sealed class EntityGroupConfiguration : IEntityTypeConfiguration<EntityGroup>
{
    public void Configure(EntityTypeBuilder<EntityGroup> builder)
    {
        builder.Configure<EntityGroup, long>();

        builder
            .Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(DataSchemaLength.Large)
            .HasComment("Наименование группы сущностей");

        builder.HasData([
            .. Enum.GetValues<EntityGroupEnum>()
                .Select(x => new EntityGroup { Id = (long)x, Name = x.ToString() }),
        ]);
    }
}
