using Edvantix.Chassis.EF.Configurations;
using Edvantix.Constants.Core;
using Edvantix.System.Domain.AggregatesModel.SubscriptionAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edvantix.System.Infrastructure.EntityConfigurations;

public sealed class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.Configure<Subscription, long>();

        builder.Property(s => s.Name).IsRequired().HasMaxLength(DataSchemaLength.ExtraLarge);

        builder
            .Property(s => s.Type)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(DataSchemaLength.Medium);

        builder.Property(s => s.Price).IsRequired().HasPrecision(18, 2);

        builder.Property(s => s.CurrencyId).IsRequired();

        builder
            .HasMany(s => s.Limits)
            .WithOne(l => l.Subscription)
            .HasForeignKey(l => l.SubscriptionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .Metadata.FindNavigation(nameof(Subscription.Limits))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(s => s.Name);
        builder.HasIndex(s => s.Type);
        builder.HasIndex(s => s.CurrencyId);
    }
}
