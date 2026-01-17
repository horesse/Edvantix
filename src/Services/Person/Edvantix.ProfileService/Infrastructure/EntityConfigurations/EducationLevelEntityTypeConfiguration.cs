using Edvantix.Chassis.EF.Configurations;
using Edvantix.Constants.Core;
using Edvantix.ProfileService.Domain.AggregatesModel.EducationAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edvantix.ProfileService.Infrastructure.EntityConfigurations;

public sealed class EducationLevelEntityTypeConfiguration : IEntityTypeConfiguration<EducationLevel>
{
    public void Configure(EntityTypeBuilder<EducationLevel> builder)
    {
        builder.ConfigureSoftDeletable<EducationLevel, long>();

        builder.Property(el => el.Name).HasMaxLength(DataSchemaLength.Large).IsRequired();

        builder.Property(el => el.Code).HasMaxLength(DataSchemaLength.Medium).IsRequired();

        builder.Property(el => el.IsDeleted).IsRequired();

        builder.HasIndex(el => el.Code);
        builder.HasIndex(el => el.IsDeleted);

        builder.HasData(
            new EducationLevel("Дошкольное образование", "preschool"),
            new EducationLevel("Общее среднее образование", "general_secondary"),
            new EducationLevel("Профессионально-техническое образование", "vocational_technical"),
            new EducationLevel("Среднее специальное образование", "secondary_specialized"),
            new EducationLevel("Высшее образование (I ступень)", "higher_bachelor"),
            new EducationLevel("Высшее образование (II ступень)", "higher_master"),
            new EducationLevel("Послевузовское образование", "postgraduate"),
            new EducationLevel(
                "Дополнительное образование детей и молодежи",
                "additional_children"
            ),
            new EducationLevel("Дополнительное образование взрослых", "additional_adults"),
            new EducationLevel("Специальное образование", "special")
        );
    }
}
