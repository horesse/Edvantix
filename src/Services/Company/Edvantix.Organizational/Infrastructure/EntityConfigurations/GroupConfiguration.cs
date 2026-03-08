using Edvantix.Chassis.EF.Configurations;

namespace Edvantix.Organizational.Infrastructure.EntityConfigurations;

internal sealed class GroupConfiguration : IEntityTypeConfiguration<Group>
{
    public void Configure(EntityTypeBuilder<Group> builder)
    {
        builder.ConfigureSoftDeletable<Group>();

        builder.Property(g => g.OrganizationId).IsRequired();

        builder.Property(g => g.Name).IsRequired().HasMaxLength(DataSchemaLength.SuperLarge);

        builder
            .Property(g => g.Description)
            .IsRequired(false)
            .HasMaxLength(DataSchemaLength.MaxText);

        builder
            .HasOne(g => g.Organization)
            .WithMany(o => o.Groups)
            .HasForeignKey(g => g.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(g => g.Members)
            .WithOne(m => m.Group)
            .HasForeignKey(m => m.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .Metadata.FindNavigation(nameof(Group.Members))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(g => g.OrganizationId);
        builder.HasIndex(g => g.Name);
    }
}
