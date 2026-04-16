using Edvantix.Chassis.EF.Configurations;
using Edvantix.Organizational.Domain.AggregatesModel.OrganizationAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edvantix.Organizational.Infrastructure.Configurations;

internal sealed class ContactConfiguration : IEntityTypeConfiguration<Contact>
{
    public void Configure(EntityTypeBuilder<Contact> builder)
    {
        builder.UseDefaultConfiguration();

        builder.Property(c => c.Value).IsRequired().HasMaxLength(DataSchemaLength.ExtraLarge);
        builder.Property(c => c.Description).HasMaxLength(DataSchemaLength.SuperLarge);
        builder
            .Property(c => c.ContactType)
            .IsRequired()
            .HasMaxLength(DataSchemaLength.Small)
            .HasConversion<string>();

        // Индекс для выборки контактов организации; уникальность primary-контакта обеспечивается доменом
        builder.HasIndex(c => new { c.OrganizationId, c.IsPrimary });
    }
}
