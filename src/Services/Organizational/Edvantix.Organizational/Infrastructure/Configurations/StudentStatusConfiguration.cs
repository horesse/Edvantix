using Edvantix.Chassis.EF.Configurations;
using Edvantix.Organizational.Domain.AggregatesModel.StudentStatusAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edvantix.Organizational.Infrastructure.Configurations;

internal sealed class StudentStatusConfiguration : IEntityTypeConfiguration<StudentStatus>
{
    public void Configure(EntityTypeBuilder<StudentStatus> builder)
    {
        builder.ConfigureSoftDeletable();

        builder.ToTable("student_statuses");

        builder.Property(s => s.OrganizationId).IsRequired();
        builder.Property(s => s.Name).IsRequired().HasMaxLength(120);
        builder.Property(s => s.Code).IsRequired().HasMaxLength(20);
        builder.Property(s => s.Tone).IsRequired().HasConversion<string>();
        builder.Property(s => s.IsSystem).IsRequired();
        builder.Property(s => s.Order).IsRequired();
        builder.Property(s => s.CreatedBy);
        builder.Property(s => s.LastModifiedBy);

        // Уникальность имени среди не удалённых записей в рамках организации
        builder
            .HasIndex(s => new { s.OrganizationId, s.Name })
            .HasFilter("is_deleted = false")
            .IsUnique()
            .HasDatabaseName("ix_student_statuses_org_name_active");

        // Уникальность кода среди не удалённых записей в рамках организации
        builder
            .HasIndex(s => new { s.OrganizationId, s.Code })
            .HasFilter("is_deleted = false")
            .IsUnique()
            .HasDatabaseName("ix_student_statuses_org_code_active");

        builder.HasIndex(s => s.OrganizationId).HasDatabaseName("ix_student_statuses_org_id");
    }
}
