using Edvantix.Chassis.EF.Configurations;
using Edvantix.OrganizationManagement.Domain.AggregatesModel.UsageAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edvantix.OrganizationManagement.Infrastructure.EntityConfigurations;

public sealed class UsageConfiguration : IEntityTypeConfiguration<Usage>
{
    public void Configure(EntityTypeBuilder<Usage> builder)
    {
        builder.Configure<Usage, long>();

        builder.Property(u => u.OrganizationId)
            .IsRequired();

        builder.Property(u => u.LimitId)
            .IsRequired();

        builder.Property(u => u.Value)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.HasOne(u => u.Organization)
            .WithMany()
            .HasForeignKey(u => u.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasIndex(u => u.OrganizationId);
        builder.HasIndex(u => u.LimitId);
        builder.HasIndex(u => new { u.OrganizationId, u.LimitId });
    }
}
