using Edvantix.Chassis.EF.Configurations;
using Edvantix.Constants.Core;
using Edvantix.Person.Domain.AggregatesModel.EmploymentHistoryAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edvantix.Person.Infrastructure.EntityConfigurations;

public sealed class EmploymentHistoryConfiguration : IEntityTypeConfiguration<EmploymentHistory>
{
    public void Configure(EntityTypeBuilder<EmploymentHistory> builder)
    {
        builder.ConfigureSoftDeletable<EmploymentHistory, long>();

        builder.Property(e => e.PersonInfoId).IsRequired();

        builder.Property(e => e.CompanyName).IsRequired().HasMaxLength(DataSchemaLength.ExtraLarge);

        builder.Property(e => e.Position).IsRequired().HasMaxLength(DataSchemaLength.ExtraLarge);

        builder.Property(e => e.StartDate).IsRequired().HasColumnType("date");

        builder.Property(e => e.EndDate).HasColumnType("date");

        builder.Property(e => e.Description).HasMaxLength(DataSchemaLength.Max);

        builder.Property(e => e.IsDeleted).IsRequired().HasDefaultValue(false);

        builder.HasIndex(e => e.PersonInfoId);

        builder.HasIndex(e => e.StartDate);

        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}
