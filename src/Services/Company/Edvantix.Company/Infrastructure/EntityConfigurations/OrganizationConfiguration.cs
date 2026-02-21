using Edvantix.Chassis.EF.Configurations;
using Edvantix.Company.Domain.AggregatesModel.OrganizationAggregate;
using Edvantix.Constants.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edvantix.Company.Infrastructure.EntityConfigurations;

public sealed class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        builder.Configure<Organization>();

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
