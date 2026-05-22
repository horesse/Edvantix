using Edvantix.Groups.Domain.AggregatesModel.SubjectAggregate;
using Edvantix.SharedKernel.Helpers;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edvantix.Groups.Infrastructure.Configurations;

internal sealed class SubjectConfiguration : IEntityTypeConfiguration<Subject>
{
    public void Configure(EntityTypeBuilder<Subject> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id).HasDefaultValueSql(UniqueIdentifierHelper.NewUuidV7);

        builder.Property(s => s.CreatedAt).HasDefaultValueSql(DateTimeHelper.SqlUtcNow);

        builder.Property(s => s.LastModifiedAt).HasDefaultValueSql(DateTimeHelper.SqlUtcNow);

        // Оптимистичная блокировка через системный столбец Postgres xmin
        builder.Property(s => s.RowVersion).IsRowVersion();

        // Код предмета хранится как строка через конвертер value object → string
        builder
            .Property(s => s.Code)
            .IsRequired()
            .HasMaxLength(10)
            .HasConversion(c => c.Value, s => SubjectCode.From(s));

        builder.Property(s => s.Name).IsRequired().HasMaxLength(120);

        builder
            .Property(s => s.Color)
            .IsRequired()
            .HasMaxLength(7);

        builder.Property(s => s.Description).IsRequired(false).HasMaxLength(500);

        builder.Property(s => s.Order).IsRequired();

        builder.Property(s => s.IsArchived).IsRequired();

        // Уникальный код в рамках организации (только среди не архивных)
        builder
            .HasIndex(s => new { s.OrganizationId, s.Code })
            .IsUnique()
            .HasFilter("is_archived = false");

        // Уникальное имя в рамках организации (только среди не архивных)
        builder
            .HasIndex(s => new { s.OrganizationId, s.Name })
            .IsUnique()
            .HasFilter("is_archived = false");

        builder.HasIndex(s => new { s.OrganizationId, s.IsArchived });
    }
}
