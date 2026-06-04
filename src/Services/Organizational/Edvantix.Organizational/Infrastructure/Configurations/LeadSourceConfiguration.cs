using Edvantix.Chassis.EF.Configurations;
using Edvantix.Organizational.Domain.AggregatesModel.LeadSourceAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edvantix.Organizational.Infrastructure.Configurations;

internal sealed class LeadSourceConfiguration : IEntityTypeConfiguration<LeadSource>
{
    public void Configure(EntityTypeBuilder<LeadSource> builder)
    {
        builder.ConfigureSoftDeletable();

        builder.ToTable("lead_sources");

        builder.Property(ls => ls.OrganizationId).IsRequired();
        builder.Property(ls => ls.Name).IsRequired().HasMaxLength(120);
        builder.Property(ls => ls.Channel).IsRequired().HasConversion<string>();
        builder.Property(ls => ls.UtmTag).HasMaxLength(60);
        builder.Property(ls => ls.Order).IsRequired();
        builder.Property(ls => ls.CreatedBy);
        builder.Property(ls => ls.LastModifiedBy);

        // Уникальность имени среди не удалённых (архивных) записей в рамках организации
        builder
            .HasIndex(ls => new { ls.OrganizationId, ls.Name })
            .HasFilter("is_deleted = false")
            .IsUnique()
            .HasDatabaseName("ix_lead_sources_org_name_active");

        builder.HasIndex(ls => ls.OrganizationId).HasDatabaseName("ix_lead_sources_org_id");
    }
}
