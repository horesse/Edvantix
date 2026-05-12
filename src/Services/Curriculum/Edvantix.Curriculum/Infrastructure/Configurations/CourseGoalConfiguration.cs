using Edvantix.Curriculum.Domain.AggregatesModel.CourseAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edvantix.Curriculum.Infrastructure.Configurations;

internal sealed class CourseGoalConfiguration : IEntityTypeConfiguration<CourseGoal>
{
    public void Configure(EntityTypeBuilder<CourseGoal> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(g => g.Text).IsRequired().HasMaxLength(256);

        builder.HasIndex(g => new { g.CourseId, g.Position }).IsUnique();
    }
}
