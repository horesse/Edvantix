using Edvantix.Chassis.EF.Configurations;
using Edvantix.Organizational.Domain.AggregatesModel.PermissionAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edvantix.Organizational.Infrastructure.Configurations;

internal sealed class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.UseDefaultConfiguration();

        builder.Property(p => p.FeatureCode).IsRequired().HasMaxLength(DataSchemaLength.Large);
        builder.Property(p => p.Code).IsRequired().HasMaxLength(DataSchemaLength.Large);
        builder.Property(p => p.Name).IsRequired().HasMaxLength(DataSchemaLength.ExtraLarge);

        // FullCode вычисляется в памяти из FeatureCode и Code — не хранится в БД.
        builder.Ignore(p => p.FullCode);

        // FK на Feature.Code (alternate key): разрешение всегда принадлежит конкретной области.
        builder
            .HasOne(p => p.Feature)
            .WithMany()
            .HasForeignKey(p => p.FeatureCode)
            .HasPrincipalKey(f => f.Code)
            .OnDelete(DeleteBehavior.Restrict);

        // Код разрешения уникален в рамках функциональной области.
        builder.HasIndex(p => new { p.FeatureCode, p.Code }).IsUnique();
    }
}
