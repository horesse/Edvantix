using Edvantix.Chassis.EF.Configurations;
using Edvantix.Constants.Core;
using Edvantix.ProfileService.Domain.AggregatesModel.FullNameAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edvantix.ProfileService.Infrastructure.EntityConfigurations;

public sealed class FullNameConfiguration : IEntityTypeConfiguration<FullName>
{
    public void Configure(EntityTypeBuilder<FullName> builder)
    {
        builder.ConfigureSoftDeletable<FullName>();

        builder.Property(f => f.ProfileId).IsRequired();

        builder.Property(f => f.FirstName).IsRequired().HasMaxLength(DataSchemaLength.Large);

        builder.Property(f => f.LastName).IsRequired().HasMaxLength(DataSchemaLength.Large);

        builder.Property(f => f.MiddleName).HasMaxLength(DataSchemaLength.Large);

        builder.HasIndex(f => f.ProfileId).IsUnique();

        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}
