using Edvantix.Persona.Domain.AggregatesModel.SkillAggregate;

namespace Edvantix.Persona.Infrastructure.EntityConfigurations;

internal sealed class SkillConfiguration : IEntityTypeConfiguration<Skill>
{
    public void Configure(EntityTypeBuilder<Skill> builder)
    {
        builder.Property(s => s.Name).IsRequired().HasMaxLength(DataSchemaLength.Large);

        // Уникальность имени обеспечивается на уровне приложения (find-or-create),
        // индекс ускоряет поиск при автодополнении
        builder.HasIndex(s => s.Name);
    }
}
