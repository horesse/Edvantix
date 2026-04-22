using Edvantix.SharedKernel.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edvantix.Notification.Infrastructure.Configurations;

internal sealed class InAppNotificationConfiguration : IEntityTypeConfiguration<InAppNotification>
{
    public void Configure(EntityTypeBuilder<InAppNotification> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(p => p.Id).HasDefaultValueSql(UniqueIdentifierHelper.NewUuidV7);

        builder.Property(p => p.ProfileId).IsRequired();

        builder.Property(p => p.Type).IsRequired();

        builder.Property(p => p.Title).HasMaxLength(DataSchemaLength.Large).IsRequired();

        builder.Property(p => p.Message).HasMaxLength(DataSchemaLength.MaxText).IsRequired();

        builder.Property(p => p.Metadata).HasMaxLength(DataSchemaLength.Max);

        builder.Property(p => p.IsRead).IsRequired();

        // Индекс для быстрой выборки уведомлений пользователя по дате (убывающий)
        builder.HasIndex(e => new { e.ProfileId, e.CreatedAt });

        // Индекс для подсчёта непрочитанных
        builder.HasIndex(e => new { e.ProfileId, e.IsRead });
    }
}
