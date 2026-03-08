using Edvantix.Chassis.EF.Configurations;

namespace Edvantix.Persona.Infrastructure.EntityConfigurations;

public sealed class FullNameConfiguration : IEntityTypeConfiguration<FullName>
{
    public void Configure(EntityTypeBuilder<FullName> builder)
    {
        builder.UseDefaultConfiguration();

        builder.Property(f => f.ProfileId).IsRequired();

        builder.Property(f => f.FirstName).IsRequired().HasMaxLength(DataSchemaLength.Large);

        builder.Property(f => f.LastName).IsRequired().HasMaxLength(DataSchemaLength.Large);

        builder.Property(f => f.MiddleName).HasMaxLength(DataSchemaLength.Large);

        builder.HasIndex(f => f.ProfileId).IsUnique();

        builder.HasQueryFilter(f => !f.Profile.IsDeleted);
    }
}
