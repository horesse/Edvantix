using Edvantix.Chassis.EF.Configurations;
using Edvantix.Organizational.Domain.AggregatesModel.GroupAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edvantix.Organizational.Infrastructure.Configurations;

internal sealed class GroupMemberConfiguration : IEntityTypeConfiguration<GroupMember>
{
    public void Configure(EntityTypeBuilder<GroupMember> builder)
    {
        builder.UseDefaultConfiguration();

        builder.Property(h => h.ExitReason).HasMaxLength(DataSchemaLength.ExtraLarge);

        builder
            .Property(m => m.Status)
            .IsRequired()
            .HasMaxLength(DataSchemaLength.Small)
            .HasConversion<string>();

        builder
            .HasOne<GroupRole>()
            .WithMany()
            .HasForeignKey(m => m.GroupRoleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(x => x.ExitedAt == null);
    }
}
