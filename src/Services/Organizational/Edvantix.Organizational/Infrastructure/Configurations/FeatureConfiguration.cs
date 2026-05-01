using Edvantix.Chassis.EF.Configurations;
using Edvantix.Organizational.Domain.AggregatesModel.PermissionAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edvantix.Organizational.Infrastructure.Configurations;

internal sealed class FeatureConfiguration : IEntityTypeConfiguration<Feature>
{
    public void Configure(EntityTypeBuilder<Feature> builder)
    {
        builder.UseDefaultConfiguration();

        builder.Property(f => f.Code).IsRequired().HasMaxLength(200);
        builder.Property(f => f.Name).IsRequired().HasMaxLength(200);

        builder
            .HasMany(f => f.Permissions)
            .WithOne(p => p.Feature)
            .HasForeignKey(p => p.FeatureId)
            .OnDelete(DeleteBehavior.Restrict);

        // Permissions всегда нужны при работе с Feature — грузим автоматически.
        builder.Navigation(f => f.Permissions).AutoInclude();

        // Код области уникален в рамках всей системы.
        builder.HasIndex(f => f.Code).IsUnique();
    }
}
