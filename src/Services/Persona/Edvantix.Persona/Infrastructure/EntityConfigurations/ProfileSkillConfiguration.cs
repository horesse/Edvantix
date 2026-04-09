using Edvantix.Chassis.EF.Configurations;

namespace Edvantix.Persona.Infrastructure.EntityConfigurations;

internal sealed class ProfileSkillConfiguration : IEntityTypeConfiguration<ProfileSkill>
{
    public void Configure(EntityTypeBuilder<ProfileSkill> builder)
    {
        builder.UseDefaultConfiguration();

        builder.Property(ps => ps.ProfileId).IsRequired();
        builder.Property(ps => ps.SkillId).IsRequired();

        builder
            .HasOne(ps => ps.Skill)
            .WithMany()
            .HasForeignKey(ps => ps.SkillId)
            // При удалении навыка из каталога — удаляем связи с профилями
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(ps => ps.ProfileId);
        builder.HasIndex(ps => ps.SkillId);
        builder.HasIndex(ps => new { ps.ProfileId, ps.SkillId }).IsUnique();

        builder.HasQueryFilter(ps => !ps.Profile.IsDeleted);
    }
}
