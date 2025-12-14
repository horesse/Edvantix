using Edvantix.Chassis.EF.Configurations;
using Edvantix.Constants.Core;
using Edvantix.Constants.Other;
using Edvantix.EntityHub.Domain.AggregatesModel.EntityTypeAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edvantix.EntityHub.Infrastructure.EntityConfigurations;

public sealed class EntityTypeConfiguration : IEntityTypeConfiguration<EntityType>
{
    public void Configure(EntityTypeBuilder<EntityType> builder)
    {
        builder.Configure<EntityType, long>();

        builder
            .Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(DataSchemaLength.Large)
            .HasComment("Наименование сущности");

        builder
            .Property(e => e.Description)
            .HasMaxLength(DataSchemaLength.Max)
            .HasComment("Описание сущности");

        builder
            .Property(e => e.MicroserviceId)
            .IsRequired()
            .HasComment("Идентификатор микросервиса");

        builder
            .Property(e => e.EntityGroupId)
            .IsRequired()
            .HasDefaultValue((long)EntityGroupEnum.Hidden)
            .HasComment("Идентификатор группы сущности");

        builder
            .HasOne(e => e.EntityGroup)
            .WithMany(e => e.Entities)
            .HasForeignKey(e => e.EntityGroupId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder
            .HasOne(e => e.Microservice)
            .WithMany()
            .HasForeignKey(e => e.MicroserviceId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
