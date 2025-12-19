using Edvantix.Chassis.EF.Configurations;
using Edvantix.Constants.Core;
using Edvantix.Person.Domain.AggregatesModel.ContactAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edvantix.Person.Infrastructure.EntityConfigurations;

public sealed class ContactConfiguration : IEntityTypeConfiguration<Contact>
{
    public void Configure(EntityTypeBuilder<Contact> builder)
    {
        builder.Configure<Contact, long>();

        builder.Property(c => c.PersonInfoId).IsRequired();

        builder.Property(c => c.Type).IsRequired().HasConversion<int>();

        builder.Property(c => c.Value).IsRequired().HasMaxLength(DataSchemaLength.ExtraLarge);

        builder.Property(c => c.Description).HasMaxLength(DataSchemaLength.SuperLarge);

        builder.Property(c => c.IsDeleted).IsRequired().HasDefaultValue(false);

        builder.HasIndex(c => c.PersonInfoId);

        builder.HasIndex(c => new
        {
            c.PersonInfoId,
            c.Type,
            c.Value,
        });

        builder.HasQueryFilter(c => !c.IsDeleted);
    }
}
