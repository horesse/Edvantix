using Edvantix.Chassis.EF.Configurations;

namespace Edvantix.Organizational.Infrastructure.EntityConfigurations;

internal sealed class ContactConfiguration : IEntityTypeConfiguration<OrganizationContact>
{
    public void Configure(EntityTypeBuilder<OrganizationContact> builder)
    {
        builder.UseDefaultConfiguration();

        builder.Property(c => c.OrganizationId).IsRequired();

        builder
            .Property(c => c.Type)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(DataSchemaLength.Medium);

        builder.Property(c => c.Value).IsRequired().HasMaxLength(DataSchemaLength.SuperLarge);

        builder.Property(c => c.Description).IsRequired(false).HasMaxLength(DataSchemaLength.Max);

        builder
            .HasOne(c => c.Organization)
            .WithMany(o => o.Contacts)
            .HasForeignKey(c => c.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(c => c.OrganizationId);
        builder.HasIndex(c => c.Type);
        builder.HasIndex(c => new { c.OrganizationId, c.Type });
    }
}
