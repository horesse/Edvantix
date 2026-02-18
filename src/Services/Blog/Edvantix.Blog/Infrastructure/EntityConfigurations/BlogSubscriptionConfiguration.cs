using Edvantix.Blog.Domain.AggregatesModel.SubscriptionAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edvantix.Blog.Infrastructure.EntityConfigurations;

/// <summary>
/// Конфигурация EF Core для сущности BlogSubscription.
/// ContentTypes хранится как int (битовая маска) для поддержки комбинаций типов контента.
/// </summary>
public sealed class BlogSubscriptionConfiguration : IEntityTypeConfiguration<BlogSubscription>
{
    public void Configure(EntityTypeBuilder<BlogSubscription> builder)
    {
        builder.ToTable("blog_subscriptions");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.UserId).IsRequired();

        // Битовая маска ContentSubscriptionType сохраняется как целое число
        builder.Property(s => s.ContentTypes).IsRequired().HasConversion<int>();

        builder.Property(s => s.CreatedAt).IsRequired();

        builder.Property(s => s.UpdatedAt).IsRequired();

        // Один пользователь — одна запись подписки
        builder.HasIndex(s => s.UserId).IsUnique();
    }
}
