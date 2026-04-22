using Edvantix.Chassis.EF.Configurations;
using Edvantix.Organizational.Domain.AggregatesModel.InvitationAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edvantix.Organizational.Infrastructure.Configurations;

internal sealed class InvitationConfiguration : IEntityTypeConfiguration<Invitation>
{
    public void Configure(EntityTypeBuilder<Invitation> builder)
    {
        builder.ConfigureSoftDeletable();

        builder
            .Property(i => i.Status)
            .IsRequired()
            .HasMaxLength(DataSchemaLength.Small)
            .HasConversion<string>();

        builder
            .Property(i => i.Type)
            .IsRequired()
            .HasMaxLength(DataSchemaLength.Small)
            .HasConversion<string>();

        builder.Property(i => i.TokenHash).IsRequired().HasMaxLength(DataSchemaLength.Large);

        builder.HasIndex(i => i.TokenHash).IsUnique();

        builder.Property(i => i.Email).HasMaxLength(DataSchemaLength.Medium);

        builder.Property(i => i.InviteeLogin).HasMaxLength(DataSchemaLength.Medium);

        builder.HasIndex(i => new { i.OrganizationId, i.Status });
    }
}
