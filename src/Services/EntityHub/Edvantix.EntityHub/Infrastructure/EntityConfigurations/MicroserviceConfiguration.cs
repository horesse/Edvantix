using Edvantix.Chassis.EF.Configurations;
using Edvantix.Constants.Core;
using Edvantix.EntityHub.Domain.AggregatesModel.MicroserviceAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edvantix.EntityHub.Infrastructure.EntityConfigurations;

public sealed class MicroserviceConfiguration : IEntityTypeConfiguration<Microservice>
{
    public void Configure(EntityTypeBuilder<Microservice> builder)
    {
        builder.Configure<Microservice, long>();

        builder
            .Property(m => m.Name)
            .IsRequired()
            .HasMaxLength(DataSchemaLength.ExtraLarge)
            .HasComment("Наименование сервиса");

        builder.HasIndex(m => m.Name).IsUnique();
    }
}
