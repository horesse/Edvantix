using Edvantix.Organizational.Domain.AggregatesModel.OrganizationAggregate;
using Edvantix.SharedKernel.Helpers;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edvantix.Organizational.Infrastructure.Configurations;

internal sealed class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(p => p.Id).HasDefaultValueSql(UniqueIdentifierHelper.NewUuidV7);

        builder.Property(p => p.CreatedAt).HasDefaultValueSql(DateTimeHelper.SqlUtcNow);

        builder.Property(p => p.LastModifiedAt).HasDefaultValueSql(DateTimeHelper.SqlUtcNow);

        builder.Property(p => p.RowVersion).IsRowVersion();

        builder
            .Property(o => o.FullLegalName)
            .IsRequired()
            .HasMaxLength(DataSchemaLength.SuperLarge);

        builder.Property(o => o.ShortName).HasMaxLength(DataSchemaLength.Large);

        builder
            .Property(o => o.LegalForm)
            .IsRequired()
            .HasMaxLength(DataSchemaLength.Medium)
            .HasConversion<string>();

        builder
            .Property(o => o.OrganizationType)
            .IsRequired()
            .HasMaxLength(DataSchemaLength.Medium)
            .HasConversion<string>();

        builder
            .Property(o => o.Status)
            .IsRequired()
            .HasMaxLength(DataSchemaLength.Small)
            .HasConversion<string>();

        builder
            .HasMany(o => o.Contacts)
            .WithOne()
            .HasForeignKey(c => c.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Navigation(e => e.Contacts).AutoInclude();

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
