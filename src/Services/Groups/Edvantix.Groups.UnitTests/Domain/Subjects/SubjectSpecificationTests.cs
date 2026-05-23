namespace Edvantix.Groups.UnitTests.Domain.Subjects;

public sealed class SubjectSpecificationTests
{
    private static readonly Guid OrgId = Guid.CreateVersion7();

    // ── SubjectListSpec (list constructor) ────────────────────────────────────

    [Test]
    public void GivenDefaultParams_WhenCreatingListSpec_ThenFiltersOutArchived()
    {
        // includeArchived = false → org + !archived
        var spec = new SubjectListSpec(OrgId, offset: 0, size: 20);

        spec.WhereExpressions.Count().ShouldBe(2);
    }

    [Test]
    public void GivenIncludeArchivedTrue_WhenCreatingListSpec_ThenOnlyOrgFilter()
    {
        var spec = new SubjectListSpec(OrgId, offset: 0, size: 20, includeArchived: true);

        spec.WhereExpressions.ShouldHaveSingleItem();
    }

    [Test]
    public void GivenSearchParam_WhenCreatingListSpec_ThenAddsSearchExpression()
    {
        var spec = new SubjectListSpec(OrgId, offset: 0, size: 20, search: "Мате");

        spec.WhereExpressions.Count().ShouldBe(3);
    }

    [Test]
    public void GivenPagination_WhenCreatingListSpec_ThenAppliesSkipAndTake()
    {
        var spec = new SubjectListSpec(OrgId, offset: 10, size: 5);

        spec.Skip.ShouldBe(10);
        spec.Take.ShouldBe(5);
    }

    [Test]
    public void GivenMatchingActiveSubject_WhenEvaluatingListSpec_ThenSubjectIsIncluded()
    {
        var spec = new SubjectListSpec(OrgId, offset: 0, size: 20);
        var subject = CreateSubject(OrgId);

        EvaluateAll(spec, subject).ShouldBeTrue();
    }

    [Test]
    public void GivenSubjectFromOtherOrg_WhenEvaluatingListSpec_ThenSubjectIsExcluded()
    {
        var spec = new SubjectListSpec(OrgId, offset: 0, size: 20);
        var subject = CreateSubject(Guid.CreateVersion7());

        EvaluateAll(spec, subject).ShouldBeFalse();
    }

    [Test]
    public void GivenArchivedSubject_WhenEvaluatingDefaultListSpec_ThenSubjectIsExcluded()
    {
        var spec = new SubjectListSpec(OrgId, offset: 0, size: 20);
        var subject = CreateSubject(OrgId);
        subject.Archive(Guid.Empty);

        EvaluateAll(spec, subject).ShouldBeFalse();
    }

    [Test]
    public void GivenArchivedSubject_WhenEvaluatingIncludeArchivedSpec_ThenSubjectIsIncluded()
    {
        var spec = new SubjectListSpec(OrgId, offset: 0, size: 20, includeArchived: true);
        var subject = CreateSubject(OrgId);
        subject.Archive(Guid.Empty);

        EvaluateAll(spec, subject).ShouldBeTrue();
    }

    // ── SubjectListSpec (count constructor) ───────────────────────────────────

    [Test]
    public void GivenCountConstructor_WhenCreatingListSpec_ThenNoSkipOrTake()
    {
        var spec = new SubjectListSpec(OrgId);

        spec.Skip.ShouldBe(0);
        spec.Take.ShouldBe(0);
    }

    // ── SubjectByNameSpec ─────────────────────────────────────────────────────

    [Test]
    public void GivenMatchingName_WhenEvaluatingByNameSpec_ThenSubjectIsIncluded()
    {
        var subject = CreateSubject(OrgId);
        var spec = new SubjectByNameSpec(OrgId, subject.Name);

        EvaluateAll(spec, subject).ShouldBeTrue();
    }

    [Test]
    public void GivenNameWithSpaces_WhenEvaluatingByNameSpec_ThenTrimmedNameIsMatched()
    {
        var subject = CreateSubject(OrgId); // Name = "Математика"
        var spec = new SubjectByNameSpec(OrgId, "  Математика  ");

        EvaluateAll(spec, subject).ShouldBeTrue();
    }

    [Test]
    public void GivenDifferentOrg_WhenEvaluatingByNameSpec_ThenSubjectIsExcluded()
    {
        var subject = CreateSubject(Guid.CreateVersion7());
        var spec = new SubjectByNameSpec(OrgId, subject.Name);

        EvaluateAll(spec, subject).ShouldBeFalse();
    }

    [Test]
    public void GivenArchivedSubject_WhenEvaluatingByNameSpec_ThenSubjectIsExcluded()
    {
        var subject = CreateSubject(OrgId);
        subject.Archive(Guid.Empty);
        var spec = new SubjectByNameSpec(OrgId, subject.Name);

        EvaluateAll(spec, subject).ShouldBeFalse();
    }

    [Test]
    public void GivenExcludeId_WhenEvaluatingByNameSpec_ThenExcludedSubjectIsNotMatched()
    {
        var subject = CreateSubject(OrgId);
        var spec = new SubjectByNameSpec(OrgId, subject.Name, subject.Id);

        EvaluateAll(spec, subject).ShouldBeFalse();
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static Subject CreateSubject(Guid orgId) =>
        new(orgId, "Математика", SubjectCode.From("MATH"), "#6366F1", null);

    /// <summary>Evaluates all WhereExpressions for the spec against the given entity (logical AND).</summary>
    private static bool EvaluateAll(Specification<Subject> spec, Subject subject) =>
        spec.WhereExpressions.Select(e => e.Filter.Compile()).All(f => f(subject));
}
