using Edvantix.Organizational.Domain.AggregatesModel.StudentTagAggregate;

namespace Edvantix.Organizational.UnitTests.Features.Directories.StudentTags.Domain;

public sealed class StudentTagTests
{
    private static readonly Guid OrgId = Guid.CreateVersion7();
    private static readonly Guid UserId = Guid.CreateVersion7();

    [Test]
    public void GivenValidParameters_WhenCreating_ThenShouldSetProperties()
    {
        var tag = new StudentTag(OrgId, "VIP", "#FF5733", order: 1, createdBy: UserId);

        tag.OrganizationId.ShouldBe(OrgId);
        tag.Name.ShouldBe("VIP");
        tag.Color.ShouldBe("#FF5733");
        tag.Order.ShouldBe(1);
        tag.CreatedBy.ShouldBe(UserId);
        tag.IsArchived.ShouldBeFalse();
    }

    [Test]
    public void GivenColorWithLowerCase_WhenCreating_ThenShouldNormalizeToUpperCase()
    {
        var tag = new StudentTag(OrgId, "VIP", "#ff5733");

        tag.Color.ShouldBe("#FF5733");
    }

    [Test]
    public void GivenColorWithLeadingWhitespace_WhenCreating_ThenShouldTrimAndNormalize()
    {
        var tag = new StudentTag(OrgId, "VIP", "  #aabbcc  ");

        tag.Color.ShouldBe("#AABBCC");
    }

    [Test]
    public void GivenNameWithWhitespace_WhenCreating_ThenShouldTrimName()
    {
        var tag = new StudentTag(OrgId, "  VIP  ", "#FF5733");

        tag.Name.ShouldBe("VIP");
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("   ")]
    public void GivenEmptyName_WhenCreating_ThenShouldThrowArgumentException(string? name)
    {
        Should.Throw<ArgumentException>(() => new StudentTag(OrgId, name!, "#FF5733"));
    }

    [Test]
    public void GivenNameExceeding40Chars_WhenCreating_ThenShouldThrowArgumentException()
    {
        var longName = new string('А', StudentTag.MaxNameLength + 1);

        Should.Throw<ArgumentException>(() => new StudentTag(OrgId, longName, "#FF5733"));
    }

    [Test]
    public void GivenNameExactly40Chars_WhenCreating_ThenShouldSucceed()
    {
        var name = new string('А', StudentTag.MaxNameLength);

        var tag = new StudentTag(OrgId, name, "#FF5733");

        tag.Name.Length.ShouldBe(StudentTag.MaxNameLength);
    }

    [Test]
    [Arguments("")]
    [Arguments("   ")]
    [Arguments("FF5733")]
    [Arguments("#GG5733")]
    [Arguments("#FF573")]
    [Arguments("#FF57330")]
    [Arguments("FF5733FF")]
    public void GivenInvalidColor_WhenCreating_ThenShouldThrowArgumentException(string color)
    {
        Should.Throw<ArgumentException>(() => new StudentTag(OrgId, "VIP", color));
    }

    [Test]
    [Arguments("#000000")]
    [Arguments("#FFFFFF")]
    [Arguments("#ff5733")]
    [Arguments("#aAbBcC")]
    public void GivenValidColor_WhenCreating_ThenShouldSucceed(string color)
    {
        Should.NotThrow(() => new StudentTag(OrgId, "VIP", color));
    }

    [Test]
    public void GivenEmptyOrganizationId_WhenCreating_ThenShouldThrowArgumentException()
    {
        Should.Throw<ArgumentException>(() => new StudentTag(Guid.Empty, "VIP", "#FF5733"));
    }

    [Test]
    public void GivenActiveTag_WhenUpdate_ThenShouldUpdateProperties()
    {
        var tag = new StudentTag(OrgId, "VIP", "#FF5733");

        tag.Update("Premium", "#0000FF", 2, UserId);

        tag.Name.ShouldBe("Premium");
        tag.Color.ShouldBe("#0000FF");
        tag.Order.ShouldBe(2);
    }

    [Test]
    public void GivenActiveTag_WhenUpdateWithSameColor_ThenShouldNotTouch()
    {
        var tag = new StudentTag(OrgId, "VIP", "#FF5733");
        var beforeModified = tag.LastModifiedAt;

        tag.Update("VIP", "#FF5733", 0, UserId);

        tag.LastModifiedAt.ShouldBe(beforeModified);
    }

    [Test]
    public void GivenActiveTag_WhenArchiving_ThenShouldSetIsArchived()
    {
        var tag = new StudentTag(OrgId, "VIP", "#FF5733");

        tag.Archive(UserId);

        tag.IsArchived.ShouldBeTrue();
    }

    [Test]
    public void GivenAlreadyArchivedTag_WhenArchiving_ThenShouldBeIdempotent()
    {
        var tag = new StudentTag(OrgId, "VIP", "#FF5733");
        tag.Archive(UserId);
        var modifiedAt = tag.LastModifiedAt;

        tag.Archive(UserId);

        tag.IsArchived.ShouldBeTrue();
        tag.LastModifiedAt.ShouldBe(modifiedAt);
    }

    [Test]
    public void GivenArchivedTag_WhenRestoring_ThenShouldClearIsArchived()
    {
        var tag = new StudentTag(OrgId, "VIP", "#FF5733");
        tag.Archive(UserId);

        tag.Restore(UserId);

        tag.IsArchived.ShouldBeFalse();
    }

    [Test]
    public void GivenActiveTag_WhenRestoring_ThenShouldBeIdempotent()
    {
        var tag = new StudentTag(OrgId, "VIP", "#FF5733");
        var modifiedAt = tag.LastModifiedAt;

        tag.Restore(UserId);

        tag.IsArchived.ShouldBeFalse();
        tag.LastModifiedAt.ShouldBe(modifiedAt);
    }
}
