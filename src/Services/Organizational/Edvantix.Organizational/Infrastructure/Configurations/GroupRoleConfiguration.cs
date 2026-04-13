using Edvantix.Chassis.EF.Configurations;
using Edvantix.Organizational.Domain.AggregatesModel.GroupAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.PermissionAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edvantix.Organizational.Infrastructure.Configurations;

internal sealed class GroupRoleConfiguration : IEntityTypeConfiguration<GroupRole>
{
    public void Configure(EntityTypeBuilder<GroupRole> builder)
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
                "group_role_permissions",
                j =>
                    j.HasOne(typeof(Permission))
                        .WithMany()
                        .HasForeignKey("permission_id")
                        .OnDelete(DeleteBehavior.Cascade),
                j =>
                    j.HasOne(typeof(GroupRole))
                        .WithMany()
                        .HasForeignKey("role_id")
                        .OnDelete(DeleteBehavior.Cascade)
            );
    }
}
