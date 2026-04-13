using Edvantix.Chassis.EF.Configurations;
using Edvantix.Organizational.Domain.AggregatesModel.GroupAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edvantix.Organizational.Infrastructure.Configurations;

internal sealed class GroupMembershipHistoryConfiguration
    : IEntityTypeConfiguration<GroupMembershipHistory>
{
    public void Configure(EntityTypeBuilder<GroupMembershipHistory> builder)
    {
        // Иммутабельный журнал: нет soft delete, нет cascade delete
        builder.UseDefaultConfiguration();

        builder.Property(h => h.ExitReason).HasMaxLength(DataSchemaLength.ExtraLarge);

        // Запись доступна только для чтения после вставки — нет Update
        builder.ToTable(tb =>
            tb.HasComment("Иммутабельный журнал вступления и выхода участников из групп.")
        );
    }
}
