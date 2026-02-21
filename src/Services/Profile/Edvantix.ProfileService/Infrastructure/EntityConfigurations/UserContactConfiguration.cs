using Edvantix.Chassis.EF.Configurations;
using Edvantix.Constants.Core;
using Edvantix.ProfileService.Domain.AggregatesModel.ContactAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edvantix.ProfileService.Infrastructure.EntityConfigurations;

public sealed class UserContactConfiguration : IEntityTypeConfiguration<UserContact>
{
    public void Configure(EntityTypeBuilder<UserContact> builder)
    {
        builder.ConfigureSoftDeletable<UserContact>();

        builder.Property(c => c.ProfileId).IsRequired();

        builder.Property(c => c.Type).IsRequired().HasConversion<int>();

        builder.Property(c => c.Value).IsRequired().HasMaxLength(DataSchemaLength.ExtraLarge);

        builder.Property(c => c.Description).HasMaxLength(DataSchemaLength.SuperLarge);

        builder.Property(c => c.IsDeleted).IsRequired().HasDefaultValue(false);

        builder.HasIndex(c => c.ProfileId);

        builder.HasIndex(c => new
        {
            PersonInfoId = c.ProfileId,
            c.Type,
            c.Value,
        });

        builder.HasQueryFilter(c => !c.IsDeleted);
    }
}
