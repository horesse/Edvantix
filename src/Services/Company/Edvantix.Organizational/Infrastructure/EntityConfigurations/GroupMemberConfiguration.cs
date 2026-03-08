using Edvantix.Chassis.EF.Configurations;

namespace Edvantix.Organizational.Infrastructure.EntityConfigurations;

internal sealed class GroupMemberConfiguration : IEntityTypeConfiguration<GroupMember>
{
    public void Configure(EntityTypeBuilder<GroupMember> builder)
    {
        builder.ConfigureSoftDeletable<GroupMember>();

        builder.Property(m => m.GroupId).IsRequired();

        builder.Property(m => m.ProfileId).IsRequired();

        builder.Property(m => m.Role).IsRequired().HasConversion<int>();

        builder.Property(m => m.JoinedAt).IsRequired();

        builder
            .HasOne(m => m.Group)
            .WithMany(g => g.Members)
            .HasForeignKey(m => m.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(m => m.GroupId);
        builder.HasIndex(m => m.ProfileId);
        builder.HasIndex(m => new
        {
            m.GroupId,
            m.ProfileId,
            m.IsDeleted,
        });
    }
}
