using Edvantix.Chassis.EF.Configurations;
using Edvantix.Groups.Domain.LessonTypeAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edvantix.Groups.Infrastructure.Configurations;

internal sealed class LessonTypeConfiguration : IEntityTypeConfiguration<LessonType>
{
    public void Configure(EntityTypeBuilder<LessonType> builder)
    {
        builder.UseDefaultConfiguration();

        builder.Property(lt => lt.OrganizationId).IsRequired();
        builder.Property(lt => lt.Name).IsRequired().HasMaxLength(120);
        builder.Property(lt => lt.Code).IsRequired().HasMaxLength(20);
        builder.Property(lt => lt.DefaultDurationMinutes).IsRequired();
        builder.Property(lt => lt.Color).IsRequired().HasMaxLength(7);
        builder.Property(lt => lt.Icon).IsRequired(false).HasMaxLength(40);
        builder.Property(lt => lt.Order).IsRequired();
        builder.Property(lt => lt.IsArchived).IsRequired();
        builder.Property(lt => lt.CreatedBy).IsRequired(false);
        builder.Property(lt => lt.LastModifiedBy).IsRequired(false);

        // Уникальное имя в рамках организации среди не архивных записей
        builder
            .HasIndex(lt => new { lt.OrganizationId, lt.Name })
            .IsUnique()
            .HasFilter("is_archived = false");

        // Уникальный код в рамках организации среди не архивных записей
        builder
            .HasIndex(lt => new { lt.OrganizationId, lt.Code })
            .IsUnique()
            .HasFilter("is_archived = false");

        builder.HasIndex(lt => new { lt.OrganizationId, lt.IsArchived });
    }
}
