using Edvantix.Chassis.EF.Configurations;
using Edvantix.Organizational.Domain.AggregatesModel.LegalFormAggregate;

namespace Edvantix.Organizational.Infrastructure.EntityConfigurations;

internal sealed class LegalFormConfiguration : IEntityTypeConfiguration<LegalForm>
{
    public void Configure(EntityTypeBuilder<LegalForm> builder)
    {
        builder.UseDefaultConfiguration<LegalForm>();

        builder.ToTable("legal_form");

        builder.Property(lf => lf.Name).IsRequired().HasMaxLength(DataSchemaLength.Large);

        builder.Property(lf => lf.ShortName).IsRequired().HasMaxLength(DataSchemaLength.Small);

        // Сокращённое наименование уникально в рамках справочника.
        builder.HasIndex(lf => lf.ShortName).IsUnique();
    }
}
