using Edvantix.Chassis.EF.Configurations;
using Edvantix.Groups.Domain.AggregatesModel.GroupAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edvantix.Groups.Infrastructure.Configurations;

internal sealed class GroupMemberConfiguration : IEntityTypeConfiguration<GroupMember>
{
    public void Configure(EntityTypeBuilder<GroupMember> builder)
    {
        builder.UseDefaultConfiguration();

        builder
            .Property(m => m.Role)
            .IsRequired()
            .HasMaxLength(DataSchemaLength.Small)
            .HasConversion<string>();

        builder.Property(m => m.ExitReason).HasMaxLength(DataSchemaLength.ExtraLarge);

        // Активные участники — у которых нет даты выхода
        builder.HasQueryFilter(x => x.ExitedAt == null);
    }
}
