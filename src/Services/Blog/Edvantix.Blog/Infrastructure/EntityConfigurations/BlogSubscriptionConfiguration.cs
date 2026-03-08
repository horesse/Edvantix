using Edvantix.Chassis.EF.Configurations;

namespace Edvantix.Blog.Infrastructure.EntityConfigurations;

internal sealed class BlogSubscriptionConfiguration : IEntityTypeConfiguration<BlogSubscription>
{
    public void Configure(EntityTypeBuilder<BlogSubscription> builder)
    {
        builder.UseDefaultConfiguration();

        builder.Property(s => s.UserId).IsRequired();

        // Битовая маска ContentSubscriptionType сохраняется как целое число
        builder.Property(s => s.ContentTypes).IsRequired().HasConversion<int>();

        builder.Property(s => s.CreatedAt).IsRequired();

        builder.Property(s => s.UpdatedAt).IsRequired();

        // Один пользователь — одна запись подписки
        builder.HasIndex(s => s.UserId).IsUnique();
    }
}
