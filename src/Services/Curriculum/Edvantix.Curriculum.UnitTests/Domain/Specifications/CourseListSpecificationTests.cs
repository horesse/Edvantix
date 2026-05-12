using Edvantix.Curriculum.Domain.AggregatesModel.CourseAggregate.Specifications;

namespace Edvantix.Curriculum.UnitTests.Domain.Specifications;

public sealed class CourseListSpecificationTests
{
    private static readonly Guid OrgId = Guid.CreateVersion7();

    private static Course CreateCourse(
        Guid? organizationId = null,
        CourseSubject subject = CourseSubject.English,
        CourseStatus? status = null
    )
    {
        var course = new Course(
            organizationId ?? OrgId,
            "EN-GEN-B1",
            "English General B1",
            subject,
            "B1",
            durationWeeks: 12,
            Guid.CreateVersion7()
        );

        if (status == CourseStatus.Active)
            course.Publish();
        else if (status == CourseStatus.Archived)
            course.Archive();

        return course;
    }

    // ─── CourseListSpecification ───────────────────────────────────────────────

    [Test]
    public void GivenOffsetAndLimit_WhenCreatingCourseListSpecification_ThenSkipAndTakeAreSet()
    {
        var spec = new CourseListSpecification(OrgId, 20, 50, null, null, null);

        spec.Skip.ShouldBe(20);
        spec.Take.ShouldBe(50);
    }

    [Test]
    public void GivenValidParameters_WhenCreatingCourseListSpecification_ThenAsNoTrackingIsTrue()
    {
        var spec = new CourseListSpecification(OrgId, 0, 10, null, null, null);

        spec.AsNoTracking.ShouldBeTrue();
    }

    [Test]
    public void GivenMatchingCourse_WhenEvaluatingCourseListSpecificationFilter_ThenCourseIsIncluded()
    {
        var spec = new CourseListSpecification(OrgId, 0, 10, null, null, null);
        var course = CreateCourse();
        var filter = spec.WhereExpressions.Single().Filter.Compile();

        filter(course).ShouldBeTrue();
    }

    [Test]
    public void GivenDifferentOrganizationId_WhenEvaluatingCourseListSpecificationFilter_ThenCourseIsExcluded()
    {
        var spec = new CourseListSpecification(OrgId, 0, 10, null, null, null);
        var course = CreateCourse(organizationId: Guid.CreateVersion7());
        var filter = spec.WhereExpressions.Single().Filter.Compile();

        filter(course).ShouldBeFalse();
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("   ")]
    public void GivenNullOrWhiteSpaceSearch_WhenCreatingCourseListSpecification_ThenNoSearchExpressionsAdded(
        string? search
    )
    {
        var spec = new CourseListSpecification(OrgId, 0, 10, search, null, null);

        spec.SearchExpressions.ShouldBeEmpty();
    }

    [Test]
    public void GivenSearchTerm_WhenCreatingCourseListSpecification_ThenTwoSearchExpressionsAreAdded()
    {
        var spec = new CourseListSpecification(OrgId, 0, 10, "english", null, null);

        spec.SearchExpressions.Count().ShouldBe(2);
    }

    [Test]
    public void GivenSearchTerm_WhenCreatingCourseListSpecification_ThenBothExpressionsAreInGroup1()
    {
        var spec = new CourseListSpecification(OrgId, 0, 10, "english", null, null);
        var expressions = spec.SearchExpressions.ToList();

        expressions.ShouldAllBe(e => e.SearchGroup == 1);
    }

    [Test]
    public void GivenSubjectFilter_WhenCreatingCourseListSpecification_ThenSubjectFilterIsApplied()
    {
        var spec = new CourseListSpecification(OrgId, 0, 10, null, CourseSubject.Math, null);
        var matchingCourse = CreateCourse(subject: CourseSubject.Math);
        var nonMatchingCourse = CreateCourse(subject: CourseSubject.English);

        var filters = spec.WhereExpressions.Select(e => e.Filter.Compile()).ToList();

        filters.All(f => f(matchingCourse)).ShouldBeTrue();
        filters.Any(f => !f(nonMatchingCourse)).ShouldBeTrue();
    }

    [Test]
    public void GivenStatusFilter_WhenCreatingCourseListSpecification_ThenStatusFilterIsApplied()
    {
        var spec = new CourseListSpecification(OrgId, 0, 10, null, null, CourseStatus.Active);
        var activeCourse = CreateCourse(status: CourseStatus.Active);
        var draftCourse = CreateCourse(status: null);

        var filters = spec.WhereExpressions.Select(e => e.Filter.Compile()).ToList();

        filters.All(f => f(activeCourse)).ShouldBeTrue();
        filters.Any(f => !f(draftCourse)).ShouldBeTrue();
    }

    [Test]
    public void GivenNoOptionalFilters_WhenCreatingCourseListSpecification_ThenOnlyOneWhereExpressionAdded()
    {
        var spec = new CourseListSpecification(OrgId, 0, 10, null, null, null);

        spec.WhereExpressions.Count().ShouldBe(1);
    }

    [Test]
    public void GivenSubjectAndStatusFilters_WhenCreatingCourseListSpecification_ThenThreeWhereExpressionsAdded()
    {
        var spec = new CourseListSpecification(
            OrgId,
            0,
            10,
            null,
            CourseSubject.English,
            CourseStatus.Draft
        );

        spec.WhereExpressions.Count().ShouldBe(3);
    }

    // ─── CourseCountSpecification ──────────────────────────────────────────────

    [Test]
    public void GivenValidParameters_WhenCreatingCourseCountSpecification_ThenNoSkipOrTake()
    {
        var spec = new CourseCountSpecification(OrgId, null, null, null);

        spec.Skip.ShouldBe(0);
        spec.Take.ShouldBe(0);
    }

    [Test]
    public void GivenMatchingCourse_WhenEvaluatingCourseCountSpecificationFilter_ThenCourseIsIncluded()
    {
        var spec = new CourseCountSpecification(OrgId, null, null, null);
        var course = CreateCourse();
        var filter = spec.WhereExpressions.Single().Filter.Compile();

        filter(course).ShouldBeTrue();
    }

    [Test]
    public void GivenDifferentOrganizationId_WhenEvaluatingCourseCountSpecificationFilter_ThenCourseIsExcluded()
    {
        var spec = new CourseCountSpecification(OrgId, null, null, null);
        var course = CreateCourse(organizationId: Guid.CreateVersion7());
        var filter = spec.WhereExpressions.Single().Filter.Compile();

        filter(course).ShouldBeFalse();
    }

    [Test]
    public void GivenSearchTerm_WhenCreatingCourseCountSpecification_ThenTwoSearchExpressionsAdded()
    {
        var spec = new CourseCountSpecification(OrgId, "math", null, null);

        spec.SearchExpressions.Count().ShouldBe(2);
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("   ")]
    public void GivenNullOrWhiteSpaceSearch_WhenCreatingCourseCountSpecification_ThenNoSearchExpressionsAdded(
        string? search
    )
    {
        var spec = new CourseCountSpecification(OrgId, search, null, null);

        spec.SearchExpressions.ShouldBeEmpty();
    }

    [Test]
    public void GivenSubjectFilter_WhenCreatingCourseCountSpecification_ThenSubjectWhereExpressionAdded()
    {
        var spec = new CourseCountSpecification(OrgId, null, CourseSubject.Kids, null);

        spec.WhereExpressions.Count().ShouldBe(2);
    }

    [Test]
    public void GivenStatusFilter_WhenCreatingCourseCountSpecification_ThenStatusWhereExpressionAdded()
    {
        var spec = new CourseCountSpecification(OrgId, null, null, CourseStatus.Archived);

        spec.WhereExpressions.Count().ShouldBe(2);
    }
}
