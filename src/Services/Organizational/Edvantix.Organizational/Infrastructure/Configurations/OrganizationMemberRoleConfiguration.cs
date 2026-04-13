using Edvantix.Chassis.EF.Configurations;
using Edvantix.Organizational.Domain.AggregatesModel.OrganizationMemberAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.PermissionAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edvantix.Organizational.Infrastructure.Configurations;

internal sealed class OrganizationMemberRoleConfiguration
    : IEntityTypeConfiguration<OrganizationMemberRole>
{
    public void Configure(EntityTypeBuilder<OrganizationMemberRole> builder)
    {
        builder.ConfigureSoftDeletable();

        builder.Property(r => r.Code).IsRequired().HasMaxLength(DataSchemaLength.Medium);
        builder.Property(r => r.Description).HasMaxLength(DataSchemaLength.Large);

        // Код роли уникален в рамках организации
        builder.HasIndex(r => new { r.OrganizationId, r.Code }).IsUnique();

        // Связь с разрешениями через промежуточную таблицу
        builder
            .HasMany(r => r.Permissions)
            .WithMany()
            .UsingEntity(
                "organization_member_role_permissions",
                j =>
                    j.HasOne(typeof(Permission))
                        .WithMany()
                        .HasForeignKey("permission_id")
                        .OnDelete(DeleteBehavior.Cascade),
                j =>
                    j.HasOne(typeof(OrganizationMemberRole))
                        .WithMany()
                        .HasForeignKey("role_id")
                        .OnDelete(DeleteBehavior.Cascade)
            );
    }
}
