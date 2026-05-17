namespace Edvantix.Groups.UnitTests.Domain.Levels;

public sealed class LevelSpecificationTests
{
    private static readonly Guid OrgId = Guid.CreateVersion7();

    // ── LevelByOrganizationSpec ────────────────────────────────────────────────

    [Test]
    public void GivenDefaultParams_WhenCreatingLevelByOrganizationSpec_ThenTwoWhereExpressionsAdded()
    {
        // includeInactive = false → org+!deleted AND IsActive
        var spec = new LevelByOrganizationSpec(OrgId);

        spec.WhereExpressions.Count().ShouldBe(2);
    }

    [Test]
    public void GivenIncludeInactiveTrue_WhenCreatingLevelByOrganizationSpec_ThenOneWhereExpressionAdded()
    {
        var spec = new LevelByOrganizationSpec(OrgId, includeInactive: true);

        spec.WhereExpressions.Count().ShouldBe(1);
    }

    [Test]
    public void GivenMatchingActiveLevel_WhenEvaluatingLevelByOrganizationSpec_ThenLevelIsIncluded()
    {
        var spec = new LevelByOrganizationSpec(OrgId);
        var level = CreateLevel(OrgId);
        var matches = EvaluateAll(spec, level);

        matches.ShouldBeTrue();
    }

    [Test]
    public void GivenLevelFromOtherOrg_WhenEvaluatingLevelByOrganizationSpec_ThenLevelIsExcluded()
    {
        var spec = new LevelByOrganizationSpec(OrgId);
        var level = CreateLevel(Guid.CreateVersion7());
        var matches = EvaluateAll(spec, level);

        matches.ShouldBeFalse();
    }

    [Test]
    public void GivenDeletedLevel_WhenEvaluatingLevelByOrganizationSpec_ThenLevelIsExcluded()
    {
        var spec = new LevelByOrganizationSpec(OrgId);
        var level = CreateLevel(OrgId);
        level.Delete();
        var matches = EvaluateAll(spec, level);

        matches.ShouldBeFalse();
    }

    [Test]
    public void GivenInactiveLevel_WhenEvaluatingDefaultSpec_ThenLevelIsExcluded()
    {
        var spec = new LevelByOrganizationSpec(OrgId, includeInactive: false);
        var level = CreateLevel(OrgId);
        level.Deactivate();
        var matches = EvaluateAll(spec, level);

        matches.ShouldBeFalse();
    }

    [Test]
    public void GivenInactiveLevel_WhenEvaluatingIncludeInactiveSpec_ThenLevelIsIncluded()
    {
        var spec = new LevelByOrganizationSpec(OrgId, includeInactive: true);
        var level = CreateLevel(OrgId);
        level.Deactivate();
        var matches = EvaluateAll(spec, level);

        matches.ShouldBeTrue();
    }

    [Test]
    public void GivenSpec_WhenCreatingLevelByOrganizationSpec_ThenOrderBySortOrderIsConfigured()
    {
        var spec = new LevelByOrganizationSpec(OrgId);

        spec.OrderExpressions.ShouldHaveSingleItem();
    }

    // ── LevelByIdsSpec ─────────────────────────────────────────────────────────

    [Test]
    public void GivenMatchingId_WhenEvaluatingLevelByIdsSpec_ThenLevelIsIncluded()
    {
        var level = CreateLevel(OrgId);
        level.Id = Guid.CreateVersion7(); // assign explicit Id for matching

        var spec = new LevelByIdsSpec([level.Id]);
        var filter = spec.WhereExpressions.Single().Filter.Compile();

        filter(level).ShouldBeTrue();
    }

    [Test]
    public void GivenNonMatchingId_WhenEvaluatingLevelByIdsSpec_ThenLevelIsExcluded()
    {
        var level = CreateLevel(OrgId);
        level.Id = Guid.CreateVersion7();

        var spec = new LevelByIdsSpec([Guid.CreateVersion7()]); // different ID
        var filter = spec.WhereExpressions.Single().Filter.Compile();

        filter(level).ShouldBeFalse();
    }

    [Test]
    public void GivenDeletedLevel_WhenEvaluatingLevelByIdsSpec_ThenLevelIsExcluded()
    {
        var level = CreateLevel(OrgId);
        level.Id = Guid.CreateVersion7();
        level.Delete();

        var spec = new LevelByIdsSpec([level.Id]);
        var filter = spec.WhereExpressions.Single().Filter.Compile();

        filter(level).ShouldBeFalse();
    }

    [Test]
    public void GivenMultipleIds_WhenCreatingLevelByIdsSpec_ThenSingleWhereExpressionAdded()
    {
        var ids = new[] { Guid.CreateVersion7(), Guid.CreateVersion7() };
        var spec = new LevelByIdsSpec(ids);

        spec.WhereExpressions.ShouldHaveSingleItem();
    }

    // ── Helpers ────────────────────────────────────────────────────────────────

    private static Level CreateLevel(Guid orgId) =>
        new(orgId, LevelCode.From("A1"), "Beginner", null, LevelTone.Blue, sortOrder: 1);

    /// <summary>Evaluates all WhereExpressions for the spec against the given entity (logical AND).</summary>
    private static bool EvaluateAll(Specification<Level> spec, Level level) =>
        spec.WhereExpressions.Select(e => e.Filter.Compile()).All(f => f(level));
}
