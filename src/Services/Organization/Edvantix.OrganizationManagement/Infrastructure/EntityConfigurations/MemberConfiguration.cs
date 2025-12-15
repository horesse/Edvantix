using Edvantix.Chassis.EF.Configurations;
using Edvantix.Constants.Core;
using Edvantix.OrganizationManagement.Domain.AggregatesModel.MemberAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edvantix.OrganizationManagement.Infrastructure.EntityConfigurations;

public sealed class MemberConfiguration : IEntityTypeConfiguration<Member>
{
    public void Configure(EntityTypeBuilder<Member> builder)
    {
        builder.ConfigureSoftDeletable<Member, Guid>();

        builder.Property(m => m.OrganizationId).IsRequired();

        builder.Property(m => m.PersonId).IsRequired();

        builder
            .Property(m => m.Position)
            .IsRequired(false)
            .HasMaxLength(DataSchemaLength.ExtraLarge);

        builder
            .HasOne(m => m.Organization)
            .WithMany(o => o.Members)
            .HasForeignKey(m => m.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(m => m.OrganizationId);
        builder.HasIndex(m => m.PersonId);
        builder.HasIndex(m => m.IsDeleted);
        builder.HasIndex(m => new
        {
            m.OrganizationId,
            m.PersonId,
            m.IsDeleted,
        });

        builder.HasQueryFilter(m => !m.IsDeleted);
    }
}
