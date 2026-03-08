using Edvantix.Chassis.EF.Configurations;

namespace Edvantix.Organizational.Infrastructure.EntityConfigurations;

internal sealed class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        builder.UseDefaultConfiguration<Organization>();

        builder.Property(o => o.Name).IsRequired().HasMaxLength(DataSchemaLength.SuperLarge);

        builder.Property(o => o.NameLatin).IsRequired().HasMaxLength(DataSchemaLength.SuperLarge);

        builder.Property(o => o.ShortName).IsRequired().HasMaxLength(DataSchemaLength.Large);

        builder
            .Property(o => o.PrintName)
            .IsRequired(false)
            .HasMaxLength(DataSchemaLength.SuperLarge);

        builder
            .Property(o => o.Description)
            .IsRequired(false)
            .HasMaxLength(DataSchemaLength.MaxText);

        builder.Property(o => o.RegistrationDate).IsRequired();

        // OrganizationType хранится как int; поле информационное, обязательное.
        builder.Property(o => o.OrganizationType).IsRequired().HasConversion<int>();

        // LegalFormId — внешний ключ на справочник организационно-правовых форм.
        builder.Property(o => o.LegalFormId).IsRequired();

        builder
            .HasOne(o => o.LegalForm)
            .WithMany()
            .HasForeignKey(o => o.LegalFormId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(o => o.LegalFormId);

        builder
            .HasMany(o => o.Contacts)
            .WithOne(c => c.Organization)
            .HasForeignKey(c => c.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(o => o.Members)
            .WithOne(m => m.Organization)
            .HasForeignKey(m => m.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(o => o.Groups)
            .WithOne(g => g.Organization)
            .HasForeignKey(g => g.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .Metadata.FindNavigation(nameof(Organization.Contacts))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder
            .Metadata.FindNavigation(nameof(Organization.Members))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder
            .Metadata.FindNavigation(nameof(Organization.Groups))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(o => o.Name);
        builder.HasIndex(o => o.NameLatin);
        builder.HasIndex(o => o.ShortName);
        builder.HasIndex(o => o.RegistrationDate);
    }
}
