using Edvantix.Chassis.EF.Configurations;

namespace Edvantix.Persona.Infrastructure.EntityConfigurations;

public sealed class EmploymentHistoryConfiguration : IEntityTypeConfiguration<EmploymentHistory>
{
    public void Configure(EntityTypeBuilder<EmploymentHistory> builder)
    {
        builder.UseDefaultConfiguration();

        builder.Property(e => e.ProfileId).IsRequired();

        builder.Property(e => e.Workplace).IsRequired().HasMaxLength(DataSchemaLength.ExtraLarge);

        builder.Property(e => e.Position).IsRequired().HasMaxLength(DataSchemaLength.ExtraLarge);

        builder.Property(e => e.StartDate).IsRequired().HasColumnType("date");

        builder.Property(e => e.EndDate).HasColumnType("date");

        builder.Property(e => e.Description).HasMaxLength(DataSchemaLength.Max);

        builder.Property(e => e.IsDeleted).IsRequired().HasDefaultValue(false);

        builder.HasIndex(e => e.ProfileId);

        builder.HasIndex(e => e.StartDate);

        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}
