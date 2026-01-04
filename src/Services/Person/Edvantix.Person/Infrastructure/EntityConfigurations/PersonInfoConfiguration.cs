using Edvantix.Chassis.EF.Configurations;
using Edvantix.Person.Domain.AggregatesModel.FullNameAggregate;
using Edvantix.Person.Domain.AggregatesModel.PersonInfoAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edvantix.Person.Infrastructure.EntityConfigurations;

public sealed class PersonInfoConfiguration : IEntityTypeConfiguration<PersonInfo>
{
    public void Configure(EntityTypeBuilder<PersonInfo> builder)
    {
        builder.ConfigureSoftDeletable<PersonInfo, long>();

        builder.Property(p => p.AccountId).IsRequired();

        builder.Property(p => p.Gender).IsRequired().HasConversion<int>();

        builder.HasIndex(p => p.AccountId).IsUnique();

        builder.HasQueryFilter(p => !p.IsDeleted);

        builder
            .HasOne(p => p.FullName)
            .WithOne(f => f.PersonInfo)
            .HasForeignKey<FullName>(f => f.PersonInfoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(p => p.Contacts)
            .WithOne(c => c.PersonInfo)
            .HasForeignKey(c => c.PersonInfoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(p => p.EmploymentHistories)
            .WithOne(e => e.PersonInfo)
            .HasForeignKey(e => e.PersonInfoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .Metadata.FindNavigation(nameof(PersonInfo.Contacts))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder
            .Metadata.FindNavigation(nameof(PersonInfo.EmploymentHistories))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
