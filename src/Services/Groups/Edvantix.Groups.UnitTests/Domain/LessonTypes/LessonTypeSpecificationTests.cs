using Edvantix.Groups.Domain.LessonTypeAggregate;
using Edvantix.Groups.Domain.LessonTypeAggregate.Specifications;

namespace Edvantix.Groups.UnitTests.Domain.LessonTypes;

public sealed class LessonTypeSpecificationTests
{
    private static readonly Guid OrgId = Guid.CreateVersion7();

    // ── LessonTypeListSpec ─────────────────────────────────────────────────────

    [Test]
    public void GivenDefaultFilters_WhenCreatingListSpec_ThenArchivedAreExcluded()
    {
        var spec = new LessonTypeListSpec(OrgId, includeArchived: false, search: null);
        var active = Build("Урок", "LESSON");
        var archived = Build("Консультация", "CONSULT");
        archived.Archive(Guid.Empty);

        var filters = CompileAll(spec);

        filters(active).ShouldBeTrue();
        filters(archived).ShouldBeFalse();
    }

    [Test]
    public void GivenIncludeArchivedTrue_WhenCreatingListSpec_ThenArchivedAreIncluded()
    {
        var spec = new LessonTypeListSpec(OrgId, includeArchived: true, search: null);
        var archived = Build("Консультация", "CONSULT");
        archived.Archive(Guid.Empty);

        CompileAll(spec)(archived).ShouldBeTrue();
    }

    [Test]
    public void GivenSearch_WhenCreatingListSpec_ThenOnlyMatchingNamesPass()
    {
        var spec = new LessonTypeListSpec(OrgId, includeArchived: false, search: "урок");
        var matching = Build("урок английского", "LESSON");
        var nonMatching = Build("Тест", "TEST");

        var filters = CompileAll(spec);

        filters(matching).ShouldBeTrue();
        filters(nonMatching).ShouldBeFalse();
    }

    [Test]
    public void GivenOtherOrg_WhenCreatingListSpec_ThenItemIsExcluded()
    {
        var spec = new LessonTypeListSpec(OrgId, includeArchived: false, search: null);
        var otherOrg = new LessonType(Guid.CreateVersion7(), "Урок", "LESSON", 45, "#3B82F6", null);

        CompileAll(spec)(otherOrg).ShouldBeFalse();
    }

    [Test]
    public void GivenPaginatedSpec_WhenCreated_ThenSkipAndTakeAreSet()
    {
        var spec = new LessonTypeListSpec(OrgId, false, null, offset: 20, limit: 10);

        spec.Skip.ShouldBe(20);
        spec.Take.ShouldBe(10);
    }

    [Test]
    public void GivenCountSpec_WhenCreated_ThenSkipAndTakeAreZero()
    {
        var spec = new LessonTypeListSpec(OrgId, false, null);

        spec.Skip.ShouldBe(0);
        spec.Take.ShouldBe(0);
    }

    [Test]
    public void GivenListSpec_WhenCreated_ThenOrderExpressionsAreConfigured()
    {
        var spec = new LessonTypeListSpec(OrgId, false, null, 0, 10);

        spec.OrderExpressions.Count().ShouldBe(2);
    }

    // ── LessonTypeUniqueNameSpec ───────────────────────────────────────────────

    [Test]
    public void GivenMatchingName_WhenEvaluatingUniqueNameSpec_ThenItemMatches()
    {
        var spec = new LessonTypeUniqueNameSpec(OrgId, "Урок");
        var item = Build("Урок", "LESSON");

        CompileAll(spec)(item).ShouldBeTrue();
    }

    [Test]
    public void GivenDifferentName_WhenEvaluatingUniqueNameSpec_ThenItemDoesNotMatch()
    {
        var spec = new LessonTypeUniqueNameSpec(OrgId, "Урок");
        var item = Build("Тест", "TEST");

        CompileAll(spec)(item).ShouldBeFalse();
    }

    [Test]
    public void GivenArchivedItem_WhenEvaluatingUniqueNameSpec_ThenItemDoesNotMatch()
    {
        var spec = new LessonTypeUniqueNameSpec(OrgId, "Урок");
        var item = Build("Урок", "LESSON");
        item.Archive(Guid.Empty);

        CompileAll(spec)(item).ShouldBeFalse();
    }

    [Test]
    public void GivenExcludeId_WhenEvaluatingUniqueNameSpec_ThenExcludedItemDoesNotMatch()
    {
        var item = Build("Урок", "LESSON");
        var spec = new LessonTypeUniqueNameSpec(OrgId, "Урок", excludeId: item.Id);

        CompileAll(spec)(item).ShouldBeFalse();
    }

    [Test]
    public void GivenOtherOrg_WhenEvaluatingUniqueNameSpec_ThenItemDoesNotMatch()
    {
        var spec = new LessonTypeUniqueNameSpec(OrgId, "Урок");
        var item = new LessonType(Guid.CreateVersion7(), "Урок", "LESSON", 45, "#3B82F6", null);

        CompileAll(spec)(item).ShouldBeFalse();
    }

    // ── LessonTypeUniqueCodeSpec ───────────────────────────────────────────────

    [Test]
    public void GivenMatchingCode_WhenEvaluatingUniqueCodeSpec_ThenItemMatches()
    {
        var spec = new LessonTypeUniqueCodeSpec(OrgId, "LESSON");
        var item = Build("Урок", "LESSON");

        CompileAll(spec)(item).ShouldBeTrue();
    }

    [Test]
    public void GivenDifferentCode_WhenEvaluatingUniqueCodeSpec_ThenItemDoesNotMatch()
    {
        var spec = new LessonTypeUniqueCodeSpec(OrgId, "LESSON");
        var item = Build("Тест", "TEST");

        CompileAll(spec)(item).ShouldBeFalse();
    }

    [Test]
    public void GivenArchivedItem_WhenEvaluatingUniqueCodeSpec_ThenItemDoesNotMatch()
    {
        var spec = new LessonTypeUniqueCodeSpec(OrgId, "LESSON");
        var item = Build("Урок", "LESSON");
        item.Archive(Guid.Empty);

        CompileAll(spec)(item).ShouldBeFalse();
    }

    [Test]
    public void GivenExcludeId_WhenEvaluatingUniqueCodeSpec_ThenExcludedItemDoesNotMatch()
    {
        var item = Build("Урок", "LESSON");
        var spec = new LessonTypeUniqueCodeSpec(OrgId, "LESSON", excludeId: item.Id);

        CompileAll(spec)(item).ShouldBeFalse();
    }

    // ── Helpers ────────────────────────────────────────────────────────────────

    private static LessonType Build(string name, string code) =>
        new(OrgId, name, code, 45, "#3B82F6", null);

    private static Func<LessonType, bool> CompileAll(Specification<LessonType> spec) =>
        item => spec.WhereExpressions.Select(e => e.Filter.Compile()).All(f => f(item));
}
