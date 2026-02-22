using Edvantix.Chassis.EF.Configurations;

namespace Edvantix.Organizational.Infrastructure.EntityConfigurations;

public sealed class OrganizationMemberConfiguration : IEntityTypeConfiguration<OrganizationMember>
{
    public void Configure(EntityTypeBuilder<OrganizationMember> builder)
    {
        builder.ConfigureSoftDeletable<OrganizationMember>();

        builder.Property(m => m.OrganizationId).IsRequired();

        builder.Property(m => m.ProfileId).IsRequired();

        builder.Property(m => m.Role).IsRequired().HasConversion<int>();

        builder.Property(m => m.JoinedAt).IsRequired();

        builder
            .HasOne(m => m.Organization)
            .WithMany(o => o.Members)
            .HasForeignKey(m => m.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(m => m.OrganizationId);
        builder.HasIndex(m => m.ProfileId);
        builder.HasIndex(m => new
        {
            m.OrganizationId,
            m.ProfileId,
            m.IsDeleted,
        });
    }
}
