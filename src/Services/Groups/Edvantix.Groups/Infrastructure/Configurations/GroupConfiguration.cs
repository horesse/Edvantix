using Edvantix.Chassis.EF.Configurations;
using Edvantix.Groups.Domain.AggregatesModel.GroupAggregate;
using Edvantix.Groups.Domain.AggregatesModel.LevelAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edvantix.Groups.Infrastructure.Configurations;

internal sealed class GroupConfiguration : IEntityTypeConfiguration<Group>
{
    public void Configure(EntityTypeBuilder<Group> builder)
    {
        builder.ConfigureSoftDeletable();

        // GroupCode хранится как строка через конвертер value object → string
        builder
            .Property(g => g.Code)
            .IsRequired()
            .HasMaxLength(32)
            .HasConversion(c => c.Value, s => GroupCode.From(s));

        builder.Property(g => g.Name).IsRequired().HasMaxLength(DataSchemaLength.Large);
        builder.Property(g => g.Description).IsRequired().HasMaxLength(DataSchemaLength.ExtraLarge);

        builder.Property(g => g.LevelId).IsRequired();

        // CourseId — логическая FK на Curriculum БД, без DB constraint (cross-service)
        builder.Property(g => g.CourseId).IsRequired();

        // TeacherMemberId — логическая FK на OrganizationMember (cross-service)
        builder.Property(g => g.TeacherMemberId).IsRequired();

        builder
            .Property(g => g.Format)
            .IsRequired()
            .HasMaxLength(DataSchemaLength.Small)
            .HasConversion<string>();

        builder.Property(g => g.RoomId).IsRequired(false);

        builder
            .Property(g => g.Platform)
            .IsRequired(false)
            .HasMaxLength(DataSchemaLength.Small)
            .HasConversion<string>();

        builder.Property(g => g.Capacity).IsRequired();

        builder
            .Property(g => g.Status)
            .IsRequired()
            .HasMaxLength(DataSchemaLength.Small)
            .HasConversion<string>();

        // FK: groups.level_id → levels.id; удаление уровня блокируется если на него ссылается хотя бы одна группа
        builder
            .HasOne(g => g.Level)
            .WithMany()
            .HasForeignKey(g => g.LevelId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Navigation(g => g.Level).AutoInclude();

        // Уникальный код в рамках организации (только среди не удалённых)
        builder
            .HasIndex(g => new { g.OrganizationId, g.Code })
            .IsUnique()
            .HasFilter("is_deleted = false");

        builder.HasIndex(g => new
        {
            g.OrganizationId,
            g.LevelId,
            g.Status,
        });
        builder.HasIndex(g => g.TeacherMemberId);
        builder.HasIndex(g => g.CourseId);

        builder
            .HasMany(g => g.Members)
            .WithOne()
            .HasForeignKey(m => m.GroupId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
