using Edvantix.Chassis.EF.Configurations;
using Edvantix.Constants.Core;
using Edvantix.System.Domain.AggregatesModel.LimitAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edvantix.System.Infrastructure.EntityConfigurations;

public sealed class LimitConfiguration : IEntityTypeConfiguration<Limit>
{
    public void Configure(EntityTypeBuilder<Limit> builder)
    {
        builder.Configure<Limit>();

        builder.Property(l => l.SubscriptionId).IsRequired();

        builder
            .Property(l => l.Type)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(DataSchemaLength.Medium);

        builder.Property(l => l.Value).IsRequired().HasPrecision(18, 2);

        builder
            .HasOne(l => l.Subscription)
            .WithMany(s => s.Limits)
            .HasForeignKey(l => l.SubscriptionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(l => l.SubscriptionId);

        builder.HasIndex(l => new { l.SubscriptionId, l.Type }).IsUnique();
    }
}
