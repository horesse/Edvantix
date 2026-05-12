using Edvantix.Curriculum.Domain.AggregatesModel.CourseAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edvantix.Curriculum.Infrastructure.Configurations;

internal sealed class ModuleConfiguration : IEntityTypeConfiguration<Module>
{
    public void Configure(EntityTypeBuilder<Module> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(m => m.Name).IsRequired().HasMaxLength(DataSchemaLength.Large);

        builder.Property(m => m.Summary).HasMaxLength(DataSchemaLength.ExtraLarge);

        builder.HasIndex(m => new { m.CourseId, m.Position }).IsUnique();

        builder
            .HasMany(m => m.Lessons)
            .WithOne()
            .HasForeignKey(l => l.ModuleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
