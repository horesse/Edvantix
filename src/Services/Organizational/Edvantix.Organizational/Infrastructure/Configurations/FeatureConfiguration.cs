using Edvantix.Chassis.EF.Configurations;
using Edvantix.Organizational.Domain.AggregatesModel.PermissionAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edvantix.Organizational.Infrastructure.Configurations;

internal sealed class FeatureConfiguration : IEntityTypeConfiguration<Feature>
{
    public void Configure(EntityTypeBuilder<Feature> builder)
    {
        builder.UseDefaultConfiguration();

        builder.Property(f => f.ServiceCode).IsRequired().HasMaxLength(DataSchemaLength.Large);
        builder.Property(f => f.Code).IsRequired().HasMaxLength(DataSchemaLength.Large);
        builder.Property(f => f.Name).IsRequired().HasMaxLength(DataSchemaLength.ExtraLarge);

        // Code используется как альтернативный ключ — на него ссылается Permission.FeatureCode.
        builder.HasAlternateKey(f => f.Code);
    }
}
