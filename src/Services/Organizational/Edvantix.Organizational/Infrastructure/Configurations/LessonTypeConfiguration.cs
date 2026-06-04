using Edvantix.Chassis.EF.Configurations;
using Edvantix.Organizational.Domain.LessonTypeAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edvantix.Organizational.Infrastructure.Configurations;

internal sealed class LessonTypeConfiguration : IEntityTypeConfiguration<LessonType>
{
    public void Configure(EntityTypeBuilder<LessonType> builder)
    {
        builder.ConfigureSoftDeletable();

        builder.Property(lt => lt.OrganizationId).IsRequired();
        builder.Property(lt => lt.Name).IsRequired().HasMaxLength(120);
        builder.Property(lt => lt.Code).IsRequired().HasMaxLength(20);
        builder.Property(lt => lt.DefaultDurationMinutes).IsRequired();
        builder.Property(lt => lt.Color).IsRequired().HasMaxLength(7);
        builder.Property(lt => lt.Icon).IsRequired(false).HasMaxLength(40);
        builder.Property(lt => lt.Order).IsRequired();
        builder.Property(lt => lt.CreatedBy).IsRequired(false);
        builder.Property(lt => lt.LastModifiedBy).IsRequired(false);

        // Уникальное имя в рамках организации среди не удалённых записей
        builder
            .HasIndex(lt => new { lt.OrganizationId, lt.Name })
            .IsUnique()
            .HasFilter("is_deleted = false");

        // Уникальный код в рамках организации среди не удалённых записей
        builder
            .HasIndex(lt => new { lt.OrganizationId, lt.Code })
            .IsUnique()
            .HasFilter("is_deleted = false");
    }
}
