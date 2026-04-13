using Edvantix.Chassis.EF.Configurations;
using Edvantix.Organizational.Domain.AggregatesModel.OrganizationAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edvantix.Organizational.Infrastructure.Configurations;

internal sealed class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        builder.ConfigureSoftDeletable();

        builder
            .Property(o => o.FullLegalName)
            .IsRequired()
            .HasMaxLength(DataSchemaLength.SuperLarge);
        builder.Property(o => o.ShortName).HasMaxLength(DataSchemaLength.Large);
        builder
            .Property(o => o.LegalForm)
            .IsRequired()
            .HasMaxLength(DataSchemaLength.Medium)
            .HasConversion<string>();
        builder
            .Property(o => o.OrganizationType)
            .IsRequired()
            .HasMaxLength(DataSchemaLength.Medium)
            .HasConversion<string>();
        builder
            .Property(o => o.Status)
            .IsRequired()
            .HasMaxLength(DataSchemaLength.Small)
            .HasConversion<string>();

        builder
            .HasMany(o => o.Contacts)
            .WithOne()
            .HasForeignKey(c => c.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
