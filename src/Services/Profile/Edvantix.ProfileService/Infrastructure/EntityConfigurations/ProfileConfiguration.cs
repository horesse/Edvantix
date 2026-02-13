using Edvantix.Chassis.EF.Configurations;
using Edvantix.ProfileService.Domain.AggregatesModel.FullNameAggregate;
using Edvantix.ProfileService.Domain.AggregatesModel.ProfileAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edvantix.ProfileService.Infrastructure.EntityConfigurations;

public sealed class ProfileConfiguration : IEntityTypeConfiguration<Profile>
{
    public void Configure(EntityTypeBuilder<Profile> builder)
    {
        builder.ConfigureSoftDeletable<Profile, long>();

        builder.Property(p => p.AccountId).IsRequired();

        builder.Property(p => p.Login).IsRequired().HasMaxLength(150);

        builder.Property(p => p.Gender).IsRequired().HasConversion<int>();

        builder.Property(p => p.BirthDate).IsRequired();

        builder.HasIndex(p => p.AccountId).IsUnique();

        builder.Property(p => p.Avatar).IsRequired(false);

        builder.HasQueryFilter(p => !p.IsDeleted);

        builder
            .HasOne(p => p.FullName)
            .WithOne(f => f.Profile)
            .HasForeignKey<FullName>(f => f.ProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(p => p.Contacts)
            .WithOne(c => c.Profile)
            .HasForeignKey(c => c.ProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(p => p.EmploymentHistories)
            .WithOne(e => e.Profile)
            .HasForeignKey(e => e.ProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .Metadata.FindNavigation(nameof(Profile.Contacts))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder
            .Metadata.FindNavigation(nameof(Profile.EmploymentHistories))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
