using Edvantix.Chassis.EF.Configurations;
using Edvantix.Constants.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edvantix.Organization.Infrastructure.EntityConfigurations;

public sealed class OrganizationConfiguration : IEntityTypeConfiguration<Domain.AggregatesModel.OrganizationAggregate.Organization>
{
    public void Configure(EntityTypeBuilder<Domain.AggregatesModel.OrganizationAggregate.Organization> builder)
    {
        builder.Configure<Domain.AggregatesModel.OrganizationAggregate.Organization, long>();

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
            .HasMany(o => o.Subscriptions)
            .WithOne(s => s.Organization)
            .HasForeignKey(s => s.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .Metadata.FindNavigation(nameof(Domain.AggregatesModel.OrganizationAggregate.Organization.Contacts))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder
            .Metadata.FindNavigation(nameof(Domain.AggregatesModel.OrganizationAggregate.Organization.Members))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder
            .Metadata.FindNavigation(nameof(Domain.AggregatesModel.OrganizationAggregate.Organization.Subscriptions))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(o => o.Name);
        builder.HasIndex(o => o.NameLatin);
        builder.HasIndex(o => o.ShortName);
        builder.HasIndex(o => o.RegistrationDate);
    }
}
