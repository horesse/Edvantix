using Edvantix.Chassis.EF.Configurations;

namespace Edvantix.Persona.Infrastructure.EntityConfigurations;

public sealed class UserContactConfiguration : IEntityTypeConfiguration<ProfileContact>
{
    public void Configure(EntityTypeBuilder<ProfileContact> builder)
    {
        builder.UseDefaultConfiguration();

        builder.Property(c => c.ProfileId).IsRequired();

        builder.Property(c => c.Type).IsRequired().HasConversion<int>();

        builder.Property(c => c.Value).IsRequired().HasMaxLength(DataSchemaLength.ExtraLarge);

        builder.Property(c => c.Description).HasMaxLength(DataSchemaLength.SuperLarge);

        builder.HasIndex(c => c.ProfileId);

        builder.HasIndex(c => new
        {
            PersonInfoId = c.ProfileId,
            c.Type,
            c.Value,
        });
    }
}
