using Edvantix.Chassis.EF.Configurations;
using Edvantix.Organizational.Domain.AggregatesModel.PaymentMethodAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edvantix.Organizational.Infrastructure.Configurations;

internal sealed class PaymentMethodConfiguration : IEntityTypeConfiguration<PaymentMethod>
{
    public void Configure(EntityTypeBuilder<PaymentMethod> builder)
    {
        builder.UseDefaultConfiguration();

        builder.ToTable("payment_methods");

        builder.Property(pm => pm.OrganizationId).IsRequired();
        builder.Property(pm => pm.Name).IsRequired().HasMaxLength(120);
        builder.Property(pm => pm.Code).IsRequired().HasMaxLength(20);
        builder.Property(pm => pm.IsCashless).IsRequired();
        builder.Property(pm => pm.RequiresContract).IsRequired();
        builder.Property(pm => pm.IsArchived).IsRequired();
        builder.Property(pm => pm.Order).IsRequired();
        builder.Property(pm => pm.CreatedBy);
        builder.Property(pm => pm.LastModifiedBy);

        // Уникальность имени среди не архивных записей в рамках организации
        builder
            .HasIndex(pm => new { pm.OrganizationId, pm.Name })
            .HasFilter("is_archived = false")
            .IsUnique()
            .HasDatabaseName("ix_payment_methods_org_name_active");

        // Уникальность кода среди не архивных записей в рамках организации
        builder
            .HasIndex(pm => new { pm.OrganizationId, pm.Code })
            .HasFilter("is_archived = false")
            .IsUnique()
            .HasDatabaseName("ix_payment_methods_org_code_active");

        builder
            .HasIndex(pm => pm.OrganizationId)
            .HasDatabaseName("ix_payment_methods_org_id");
    }
}
