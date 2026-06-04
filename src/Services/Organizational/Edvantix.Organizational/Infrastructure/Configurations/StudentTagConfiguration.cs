using Edvantix.Chassis.EF.Configurations;
using Edvantix.Organizational.Domain.AggregatesModel.StudentTagAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edvantix.Organizational.Infrastructure.Configurations;

internal sealed class StudentTagConfiguration : IEntityTypeConfiguration<StudentTag>
{
    public void Configure(EntityTypeBuilder<StudentTag> builder)
    {
        builder.ConfigureSoftDeletable();

        builder.ToTable("student_tags");

        builder.Property(t => t.OrganizationId).IsRequired();
        builder.Property(t => t.Name).IsRequired().HasMaxLength(40);
        builder.Property(t => t.Color).IsRequired().HasMaxLength(7);
        builder.Property(t => t.Order).IsRequired();
        builder.Property(t => t.CreatedBy);
        builder.Property(t => t.LastModifiedBy);

        // Уникальность имени среди не удалённых записей в рамках организации
        builder
            .HasIndex(t => new { t.OrganizationId, t.Name })
            .HasFilter("is_deleted = false")
            .IsUnique()
            .HasDatabaseName("ix_student_tags_org_name_active");

        builder.HasIndex(t => t.OrganizationId).HasDatabaseName("ix_student_tags_org_id");
    }
}
