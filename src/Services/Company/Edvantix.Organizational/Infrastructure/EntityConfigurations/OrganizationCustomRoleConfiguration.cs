using Edvantix.Chassis.EF.Configurations;
using Edvantix.Organizational.Domain.AggregatesModel.OrganizationCustomRoleAggregate;

namespace Edvantix.Organizational.Infrastructure.EntityConfigurations;

internal sealed class OrganizationCustomRoleConfiguration
    : IEntityTypeConfiguration<OrganizationCustomRole>
{
    public void Configure(EntityTypeBuilder<OrganizationCustomRole> builder)
    {
        builder.ConfigureSoftDeletable<OrganizationCustomRole>();

        builder.Property(r => r.OrganizationId).IsRequired();

        builder.Property(r => r.Code).IsRequired().HasMaxLength(50);

        builder.Property(r => r.Description).IsRequired(false).HasMaxLength(100);

        // BaseRole хранится как smallint (аналогично другим enum-полям в проекте).
        builder.Property(r => r.BaseRole).IsRequired().HasConversion<short>();

        builder
            .HasOne(r => r.Organization)
            .WithMany()
            .HasForeignKey(r => r.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(r => r.OrganizationId);

        // Уникальный частичный индекс: код роли уникален в рамках организации среди не удалённых записей.
        builder
            .HasIndex(r => new { r.OrganizationId, r.Code })
            .IsUnique()
            .HasFilter("NOT is_deleted");
    }
}
