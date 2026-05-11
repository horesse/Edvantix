using Edvantix.Chassis.EF.Configurations;
using Edvantix.Organizational.Domain.AggregatesModel.GroupAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edvantix.Organizational.Infrastructure.Configurations;

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

        builder
            .Property(g => g.Level)
            .IsRequired()
            .HasMaxLength(DataSchemaLength.Small)
            .HasConversion<string>();

        // CourseId — логическая FK на Curriculum БД, без constraint
        builder.Property(g => g.CourseId).IsRequired();

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

        // Уникальный код в рамках организации (только среди не удалённых)
        builder
            .HasIndex(g => new { g.OrganizationId, g.Code })
            .IsUnique()
            .HasFilter("is_deleted = false");

        builder.HasIndex(g => new { g.OrganizationId, g.Status });
        builder.HasIndex(g => g.TeacherMemberId);
        builder.HasIndex(g => g.CourseId);

        // TeacherMemberId — ссылка на OrganizationMember; FK constraint применяется на уровне приложения.
        // EF-навигация не настроена, т.к. агрегатные границы не позволяют прямой ссылки.

        builder
            .HasMany(g => g.Members)
            .WithOne()
            .HasForeignKey(m => m.GroupId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
