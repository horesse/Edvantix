using Edvantix.Chassis.EF.Configurations;
using Edvantix.Organizational.Domain.AggregatesModel.OrganizationMemberAggregate;
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

        // Индекс для быстрой выборки участников организации; уникальность активного членства обеспечивается доменом
        builder.HasIndex(m => new { m.OrganizationId, m.ProfileId });

        builder
            .HasOne<OrganizationMemberRole>()
            .WithMany()
            .HasForeignKey(m => m.OrganizationMemberRoleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
