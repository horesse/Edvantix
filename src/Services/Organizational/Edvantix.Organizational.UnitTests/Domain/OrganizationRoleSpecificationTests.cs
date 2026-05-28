using Edvantix.Organizational.Domain.AggregatesModel.OrganizationRoleAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.OrganizationRoleAggregate.Specifications;

namespace Edvantix.Organizational.UnitTests.Domain;

public sealed class OrganizationRoleSpecificationTests
{
    private static readonly Guid OrgId = Guid.CreateVersion7();

    private static OrganizationRole CreateRole(Guid? organizationId = null, bool deleted = false)
    {
        var role = new OrganizationRole(organizationId ?? OrgId, "Тестовая роль");
        if (deleted)
        {
            role.Delete();
        }

        return role;
    }

    // ── OrganizationRoleSpecification ──────────────────────────────────────────

    [Test]
    public void GivenValidParameters_WhenCreatingOrganizationRoleSpecification_ThenAsNoTrackingIsTrue()
    {
        var spec = new OrganizationRoleSpecification(OrgId, 0, 10);

        spec.AsNoTracking.ShouldBeTrue();
    }

    [Test]
    public void GivenOffsetAndLimit_WhenCreatingOrganizationRoleSpecification_ThenSkipAndTakeAreSet()
    {
        var spec = new OrganizationRoleSpecification(OrgId, 20, 50);

        spec.Skip.ShouldBe(20);
        spec.Take.ShouldBe(50);
    }

    [Test]
    public void GivenValidParameters_WhenCreatingOrganizationRoleSpecification_ThenIncludePermissionsIsConfigured()
    {
        var spec = new OrganizationRoleSpecification(OrgId, 0, 10);

        spec.IncludeExpressions.ShouldHaveSingleItem();
    }

    [Test]
    public void GivenMatchingRole_WhenEvaluatingOrganizationRoleSpecificationFilter_ThenRoleIsIncluded()
    {
        var spec = new OrganizationRoleSpecification(OrgId, 0, 10);
        var role = CreateRole();
        var filter = spec.WhereExpressions.Single().Filter.Compile();

        filter(role).ShouldBeTrue();
    }

    [Test]
    public void GivenDifferentOrganizationId_WhenEvaluatingOrganizationRoleSpecificationFilter_ThenRoleIsExcluded()
    {
        var spec = new OrganizationRoleSpecification(OrgId, 0, 10);
        var role = CreateRole(organizationId: Guid.CreateVersion7());
        var filter = spec.WhereExpressions.Single().Filter.Compile();

        filter(role).ShouldBeFalse();
    }

    [Test]
    public void GivenDeletedRole_WhenEvaluatingOrganizationRoleSpecificationFilter_ThenRoleIsExcluded()
    {
        var spec = new OrganizationRoleSpecification(OrgId, 0, 10);
        var role = CreateRole(deleted: true);
        var filter = spec.WhereExpressions.Single().Filter.Compile();

        filter(role).ShouldBeFalse();
    }

    [Test]
    public void GivenNullSearch_WhenCreatingOrganizationRoleSpecification_ThenNoSearchExpressionsAdded()
    {
        var spec = new OrganizationRoleSpecification(OrgId, 0, 10, search: null);

        spec.SearchExpressions.ShouldBeEmpty();
    }

    [Test]
    public void GivenSearchTerm_WhenCreatingOrganizationRoleSpecification_ThenTwoSearchExpressionsAreAdded()
    {
        var spec = new OrganizationRoleSpecification(OrgId, 0, 10, "тест");

        spec.SearchExpressions.Count().ShouldBe(2);
    }

    [Test]
    public void GivenSearchTerm_WhenCreatingOrganizationRoleSpecification_ThenSearchExpressionsHaveCorrectTermAndGroups()
    {
        const string search = "администратор";
        var spec = new OrganizationRoleSpecification(OrgId, 0, 10, search);
        var expressions = spec.SearchExpressions.ToList();

        expressions.ShouldAllBe(e => e.SearchTerm == search);
        expressions.ShouldContain(e => e.SearchGroup == 1); // Name
        expressions.ShouldContain(e => e.SearchGroup == 2); // Description
    }

    // ── RoleCountSpecification ─────────────────────────────────────────────────

    [Test]
    public void GivenValidParameters_WhenCreatingRoleCountSpecification_ThenAsNoTrackingIsTrue()
    {
        var spec = new RoleCountSpecification(OrgId);

        spec.AsNoTracking.ShouldBeTrue();
    }

    [Test]
    public void GivenValidParameters_WhenCreatingRoleCountSpecification_ThenNoIncludeExpressions()
    {
        var spec = new RoleCountSpecification(OrgId);

        spec.IncludeExpressions.ShouldBeEmpty();
    }

    [Test]
    public void GivenMatchingRole_WhenEvaluatingRoleCountSpecificationFilter_ThenRoleIsIncluded()
    {
        var spec = new RoleCountSpecification(OrgId);
        var role = CreateRole();
        var filter = spec.WhereExpressions.Single().Filter.Compile();

        filter(role).ShouldBeTrue();
    }

    [Test]
    public void GivenDifferentOrganizationId_WhenEvaluatingRoleCountSpecificationFilter_ThenRoleIsExcluded()
    {
        var spec = new RoleCountSpecification(OrgId);
        var role = CreateRole(organizationId: Guid.CreateVersion7());
        var filter = spec.WhereExpressions.Single().Filter.Compile();

        filter(role).ShouldBeFalse();
    }

    [Test]
    public void GivenDeletedRole_WhenEvaluatingRoleCountSpecificationFilter_ThenRoleIsExcluded()
    {
        var spec = new RoleCountSpecification(OrgId);
        var role = CreateRole(deleted: true);
        var filter = spec.WhereExpressions.Single().Filter.Compile();

        filter(role).ShouldBeFalse();
    }

    [Test]
    public void GivenNullSearch_WhenCreatingRoleCountSpecification_ThenNoSearchExpressionsAdded()
    {
        var spec = new RoleCountSpecification(OrgId, search: null);

        spec.SearchExpressions.ShouldBeEmpty();
    }

    [Test]
    public void GivenSearchTerm_WhenCreatingRoleCountSpecification_ThenTwoSearchExpressionsAreAdded()
    {
        var spec = new RoleCountSpecification(OrgId, "тест");

        spec.SearchExpressions.Count().ShouldBe(2);
    }

    [Test]
    public void GivenSearchTerm_WhenCreatingRoleCountSpecification_ThenSearchExpressionsHaveCorrectTermAndGroups()
    {
        const string search = "администратор";
        var spec = new RoleCountSpecification(OrgId, search);
        var expressions = spec.SearchExpressions.ToList();

        expressions.ShouldAllBe(e => e.SearchTerm == search);
        expressions.ShouldContain(e => e.SearchGroup == 1); // Name
        expressions.ShouldContain(e => e.SearchGroup == 2); // Description
    }
}
