using Edvantix.Chassis.EF.Configurations;
using Edvantix.Company.Domain.AggregatesModel.InvitationAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edvantix.Company.Infrastructure.EntityConfigurations;

/// <summary>
/// Конфигурация EF Core для сущности <see cref="Invitation"/>.
/// Без soft delete — статусный lifecycle (Pending → Accepted/Declined/Cancelled/Expired).
/// </summary>
public sealed class InvitationConfiguration : IEntityTypeConfiguration<Invitation>
{
    public void Configure(EntityTypeBuilder<Invitation> builder)
    {
        builder.Configure<Invitation>();

        builder.Property(x => x.OrganizationId).IsRequired();

        builder.Property(x => x.InvitedByProfileId).IsRequired();

        builder.Property(x => x.InviteeProfileId);

        builder.Property(x => x.InviteeEmail).HasMaxLength(256);

        builder.Property(x => x.Role).IsRequired().HasConversion<int>();

        builder.Property(x => x.Status).IsRequired().HasConversion<int>();

        builder.Property(x => x.Token).IsRequired();

        builder.Property(x => x.CreatedAt).IsRequired();

        builder.Property(x => x.ExpiresAt).IsRequired();

        builder.Property(x => x.RespondedAt);

        // Уникальный индекс по токену для быстрого поиска accept/decline.
        builder.HasIndex(x => x.Token).IsUnique();

        // Составной индекс для поиска ожидающих приглашений по организации.
        builder.HasIndex(x => new { x.OrganizationId, x.Status });

        // Индекс для поиска приглашений по профилю приглашённого.
        builder.HasIndex(x => new { x.InviteeProfileId, x.Status });

        // Индекс для поиска дубликатов по email.
        builder.HasIndex(x => new
        {
            x.OrganizationId,
            x.InviteeEmail,
            x.Status,
        });
    }
}
