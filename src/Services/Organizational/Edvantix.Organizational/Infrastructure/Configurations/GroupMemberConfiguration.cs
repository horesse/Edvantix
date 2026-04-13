using Edvantix.Chassis.EF.Configurations;
using Edvantix.Organizational.Domain.AggregatesModel.GroupAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edvantix.Organizational.Infrastructure.Configurations;

internal sealed class GroupMemberConfiguration : IEntityTypeConfiguration<GroupMember>
{
    public void Configure(EntityTypeBuilder<GroupMember> builder)
    {
        builder.ConfigureSoftDeletable();

        builder
            .Property(m => m.Status)
            .IsRequired()
            .HasMaxLength(DataSchemaLength.Small)
            .HasConversion<string>();

        builder
            .HasMany(m => m.History)
            .WithOne()
            .HasForeignKey(h => h.GroupMemberId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne<GroupRole>()
            .WithMany()
            .HasForeignKey(m => m.GroupRoleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
