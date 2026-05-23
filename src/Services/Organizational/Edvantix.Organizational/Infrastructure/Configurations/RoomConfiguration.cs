using Edvantix.Chassis.EF.Configurations;
using Edvantix.Organizational.Domain.AggregatesModel.RoomAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edvantix.Organizational.Infrastructure.Configurations;

internal sealed class RoomConfiguration : IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> builder)
    {
        builder.UseDefaultConfiguration();

        builder.ToTable("rooms");

        builder.Property(r => r.OrganizationId).IsRequired();
        builder.Property(r => r.Name).IsRequired().HasMaxLength(120);
        builder.Property(r => r.Capacity).IsRequired();
        builder.Property(r => r.Floor).HasMaxLength(10);
        builder.Property(r => r.RoomType).IsRequired().HasConversion<string>();
        builder.Property(r => r.IsArchived).IsRequired();
        builder.Property(r => r.Order).IsRequired();
        builder.Property(r => r.CreatedBy);
        builder.Property(r => r.LastModifiedBy);

        // Уникальность имени среди не архивных записей в рамках организации
        builder
            .HasIndex(r => new { r.OrganizationId, r.Name })
            .HasFilter("is_archived = false")
            .IsUnique()
            .HasDatabaseName("ix_rooms_org_name_active");

        builder.HasIndex(r => r.OrganizationId).HasDatabaseName("ix_rooms_org_id");
    }
}
