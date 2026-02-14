using Edvantix.Chassis.EF.Configurations;
using Edvantix.Company.Domain.AggregatesModel.GroupAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edvantix.Company.Infrastructure.EntityConfigurations;

public sealed class GroupMemberConfiguration : IEntityTypeConfiguration<GroupMember>
{
    public void Configure(EntityTypeBuilder<GroupMember> builder)
    {
        builder.ConfigureSoftDeletable<GroupMember, Guid>();

        builder.Property(m => m.GroupId).IsRequired();

        builder.Property(m => m.ProfileId).IsRequired();

        builder
            .Property(m => m.Role)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(m => m.JoinedAt).IsRequired();

        builder
            .HasOne(m => m.Group)
            .WithMany(g => g.Members)
            .HasForeignKey(m => m.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(m => m.GroupId);
        builder.HasIndex(m => m.ProfileId);
        builder.HasIndex(m => new { m.GroupId, m.ProfileId, m.IsDeleted });
    }
}
