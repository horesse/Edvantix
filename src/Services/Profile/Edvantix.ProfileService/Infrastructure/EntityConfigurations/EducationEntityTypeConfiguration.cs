using Edvantix.Chassis.EF.Configurations;
using Edvantix.Constants.Core;
using Edvantix.ProfileService.Domain.AggregatesModel.EducationAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edvantix.ProfileService.Infrastructure.EntityConfigurations;

public sealed class EducationEntityTypeConfiguration : IEntityTypeConfiguration<Education>
{
    public void Configure(EntityTypeBuilder<Education> builder)
    {
        builder.ConfigureSoftDeletable<Education>();

        builder.Property(e => e.DateStart).IsRequired();

        builder.Property(e => e.DateEnd);

        builder.Property(e => e.Institution).HasMaxLength(DataSchemaLength.ExtraLarge).IsRequired();

        builder.Property(e => e.Specialty).HasMaxLength(DataSchemaLength.Large);

        builder.Property(e => e.EducationLevel).IsRequired();

        builder.Property(e => e.ProfileId).IsRequired();

        builder.Property(e => e.IsDeleted).IsRequired();

        builder.HasIndex(e => e.ProfileId);
        builder.HasIndex(e => e.IsDeleted);
    }
}
