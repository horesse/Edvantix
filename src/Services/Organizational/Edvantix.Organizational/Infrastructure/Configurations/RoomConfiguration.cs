using Edvantix.Chassis.EF.Configurations;
using Edvantix.Organizational.Domain.AggregatesModel.RoomAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edvantix.Organizational.Infrastructure.Configurations;

internal sealed class RoomConfiguration : IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> builder)
    {
        builder.ConfigureSoftDeletable();

        builder.Property(r => r.Label).IsRequired().HasMaxLength(64);
        builder.Property(r => r.Floor).IsRequired();
        builder.Property(r => r.Seats).IsRequired();

        builder.HasIndex(r => r.OrganizationId).HasFilter("is_deleted = false");
    }
}
