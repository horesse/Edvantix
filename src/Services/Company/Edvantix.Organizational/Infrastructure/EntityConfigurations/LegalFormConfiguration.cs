using Edvantix.Chassis.EF.Configurations;
using Edvantix.Constants.Core;
using Edvantix.Organizational.Domain.AggregatesModel.LegalFormAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edvantix.Organizational.Infrastructure.EntityConfigurations;

public sealed class LegalFormConfiguration : IEntityTypeConfiguration<LegalForm>
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
