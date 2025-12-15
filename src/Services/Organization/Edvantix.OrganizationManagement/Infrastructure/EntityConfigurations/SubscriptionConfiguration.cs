using Edvantix.Chassis.EF.Configurations;
using Edvantix.OrganizationManagement.Domain.AggregatesModel.SubscriptionAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edvantix.OrganizationManagement.Infrastructure.EntityConfigurations;

public sealed class OrganizationSubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.Configure<Subscription, long>();

        builder.Property(s => s.SubscriptionId).IsRequired();

        builder.Property(s => s.OrganizationId).IsRequired();

        builder.Property(s => s.DateStart).IsRequired();

        builder.Property(s => s.DateEnd).IsRequired(false);

        builder
            .HasOne(s => s.Organization)
            .WithMany(o => o.Subscriptions)
            .HasForeignKey(s => s.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(s => s.Usages)
            .WithOne()
            .HasForeignKey(u => u.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .Metadata.FindNavigation(nameof(Subscription.Usages))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(s => s.SubscriptionId);
        builder.HasIndex(s => s.OrganizationId);
        builder.HasIndex(s => new
        {
            s.OrganizationId,
            s.DateStart,
            s.DateEnd,
        });
        builder.HasIndex(s => s.DateEnd);
    }
}
