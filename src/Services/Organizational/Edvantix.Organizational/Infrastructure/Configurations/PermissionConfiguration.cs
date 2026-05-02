using Edvantix.Chassis.EF.Configurations;
using Edvantix.Organizational.Domain.AggregatesModel.PermissionAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edvantix.Organizational.Infrastructure.Configurations;

internal sealed class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.UseDefaultConfiguration();

        builder.Property(p => p.ServiceCode).IsRequired().HasMaxLength(100);
        builder.Property(p => p.FeatureCode).IsRequired().HasMaxLength(200);
        builder.Property(p => p.FeatureName).IsRequired().HasMaxLength(200);
        builder.Property(p => p.Code).IsRequired().HasMaxLength(200);
        builder.Property(p => p.Name).IsRequired().HasMaxLength(200);

        // FullCode вычисляется в памяти из FeatureCode и Code — не хранится в БД.
        builder.Ignore(p => p.FullCode);

        // Код разрешения уникален в рамках сервиса и функциональной области.
        builder
            .HasIndex(p => new
            {
                p.ServiceCode,
                p.FeatureCode,
                p.Code,
            })
            .IsUnique();
    }
}
