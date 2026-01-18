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
            new EducationLevel(1, "Дошкольное образование", "preschool"),
            new EducationLevel(2, "Общее среднее образование", "general_secondary"),
            new EducationLevel(
                3,
                "Профессионально-техническое образование",
                "vocational_technical"
            ),
            new EducationLevel(4, "Среднее специальное образование", "secondary_specialized"),
            new EducationLevel(5, "Высшее образование (I ступень)", "higher_bachelor"),
            new EducationLevel(6, "Высшее образование (II ступень)", "higher_master"),
            new EducationLevel(7, "Послевузовское образование", "postgraduate"),
            new EducationLevel(
                8,
                "Дополнительное образование детей и молодежи",
                "additional_children"
            ),
            new EducationLevel(9, "Дополнительное образование взрослых", "additional_adults"),
            new EducationLevel(10, "Специальное образование", "special")
        );
    }
}
