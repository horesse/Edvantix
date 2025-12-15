using Edvantix.Chassis.EF.Configurations;
using Edvantix.Constants.Core;
using Edvantix.OrganizationManagement.Domain.AggregatesModel.ContactAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edvantix.OrganizationManagement.Infrastructure.EntityConfigurations;

public sealed class ContactConfiguration : IEntityTypeConfiguration<Contact>
{
    public void Configure(EntityTypeBuilder<Contact> builder)
    {
        builder.Configure<Contact, long>();

        builder.Property(c => c.OrganizationId)
            .IsRequired();

        builder.Property(c => c.Type)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(DataSchemaLength.Medium);

        builder.Property(c => c.Value)
            .IsRequired()
            .HasMaxLength(DataSchemaLength.SuperLarge);

        builder.Property(c => c.Description)
            .IsRequired(false)
            .HasMaxLength(DataSchemaLength.Max);

        builder.HasOne(c => c.Organization)
            .WithMany(o => o.Contacts)
            .HasForeignKey(c => c.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(c => c.OrganizationId);
        builder.HasIndex(c => c.Type);
        builder.HasIndex(c => new { c.OrganizationId, c.Type });
    }
}
