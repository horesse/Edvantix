using Edvantix.Chassis.EF.Configurations;
using Edvantix.Organizational.Domain.AggregatesModel.OrganizationMemberAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.OrganizationRoleAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edvantix.Organizational.Infrastructure.Configurations;

internal sealed class OrganizationMemberConfiguration : IEntityTypeConfiguration<OrganizationMember>
{
    public void Configure(EntityTypeBuilder<OrganizationMember> builder)
    {
        builder.ConfigureSoftDeletable();

        builder
            .Property(m => m.Status)
            .IsRequired()
            .HasMaxLength(DataSchemaLength.Small)
            .HasConversion<string>();

        builder.HasIndex(m => new { m.OrganizationId, m.ProfileId });

        builder
            .HasOne<OrganizationRole>(m => m.Role)
            .WithMany()
            .HasForeignKey(m => m.OrganizationRoleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Navigation(m => m.Role).AutoInclude();
    }
}
