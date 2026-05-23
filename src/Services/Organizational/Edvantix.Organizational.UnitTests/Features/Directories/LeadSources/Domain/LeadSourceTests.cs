namespace Edvantix.Organizational.UnitTests.Features.Directories.LeadSources.Domain;

public sealed class LeadSourceTests
{
    private static readonly Guid OrgId = Guid.CreateVersion7();
    private static readonly Guid UserId = Guid.CreateVersion7();

    [Test]
    public void GivenValidParams_WhenCreating_ThenShouldSetProperties()
    {
        var source = new LeadSource(OrgId, "Инстаграм", LeadChannel.Online, "utm_insta", 1, UserId);

        source.OrganizationId.ShouldBe(OrgId);
        source.Name.ShouldBe("Инстаграм");
        source.Channel.ShouldBe(LeadChannel.Online);
        source.UtmTag.ShouldBe("utm_insta");
        source.Order.ShouldBe(1);
        source.IsArchived.ShouldBeFalse();
    }

    [Test]
    public void GivenNullUtmTag_WhenCreating_ThenShouldBeAllowed()
    {
        var source = new LeadSource(OrgId, "Флаер", LeadChannel.Offline, null);

        source.UtmTag.ShouldBeNull();
    }

    [Test]
    public void GivenUtmTagWithSpaces_WhenCreating_ThenShouldTrimUtmTag()
    {
        var source = new LeadSource(OrgId, "Рекомендация", LeadChannel.Referral, "  utm_ref  ");

        source.UtmTag.ShouldBe("utm_ref");
    }

    [Test]
    public void GivenUtmTagExceedingMaxLength_WhenCreating_ThenShouldThrow()
    {
        var longTag = new string('x', LeadSource.MaxUtmTagLength + 1);

        Should.Throw<ArgumentException>(() =>
            new LeadSource(OrgId, "Источник", LeadChannel.Online, longTag)
        );
    }

    [Test]
    [Arguments("")]
    [Arguments("   ")]
    public void GivenEmptyName_WhenCreating_ThenShouldThrow(string name)
    {
        Should.Throw<Exception>(() => new LeadSource(OrgId, name, LeadChannel.Direct, null));
    }

    [Test]
    public void GivenNameExceeding120Chars_WhenCreating_ThenShouldThrow()
    {
        var longName = new string('А', 121);

        Should.Throw<Exception>(() => new LeadSource(OrgId, longName, LeadChannel.Other, null));
    }

    [Test]
    public void GivenValidUpdateParams_WhenUpdating_ThenShouldUpdateProperties()
    {
        var source = new LeadSource(OrgId, "Инстаграм", LeadChannel.Online, null, 0, UserId);

        source.Update("ВКонтакте", LeadChannel.Online, "utm_vk", 2, UserId);

        source.Name.ShouldBe("ВКонтакте");
        source.Channel.ShouldBe(LeadChannel.Online);
        source.UtmTag.ShouldBe("utm_vk");
        source.Order.ShouldBe(2);
    }

    [Test]
    public void GivenUpdate_WhenUtmTagTooLong_ThenShouldThrow()
    {
        var source = new LeadSource(OrgId, "Инстаграм", LeadChannel.Online, null);
        var longTag = new string('x', LeadSource.MaxUtmTagLength + 1);

        Should.Throw<ArgumentException>(() =>
            source.Update("Инстаграм", LeadChannel.Online, longTag, 0, UserId)
        );
    }

    [Test]
    public void GivenActiveSource_WhenArchiving_ThenIsArchivedShouldBeTrue()
    {
        var source = new LeadSource(OrgId, "Флаер", LeadChannel.Offline, null);

        source.Archive(UserId);

        source.IsArchived.ShouldBeTrue();
    }

    [Test]
    public void GivenAlreadyArchivedSource_WhenArchivingAgain_ThenShouldBeIdempotent()
    {
        var source = new LeadSource(OrgId, "Флаер", LeadChannel.Offline, null);
        source.Archive(UserId);

        source.Archive(UserId);

        source.IsArchived.ShouldBeTrue();
    }

    [Test]
    public void GivenArchivedSource_WhenRestoring_ThenIsArchivedShouldBeFalse()
    {
        var source = new LeadSource(OrgId, "Флаер", LeadChannel.Offline, null);
        source.Archive(UserId);

        source.Restore(UserId);

        source.IsArchived.ShouldBeFalse();
    }

    [Test]
    public void GivenActiveSource_WhenRestoringAgain_ThenShouldBeIdempotent()
    {
        var source = new LeadSource(OrgId, "Флаер", LeadChannel.Offline, null);

        source.Restore(UserId);

        source.IsArchived.ShouldBeFalse();
    }

    [Test]
    [Arguments(LeadChannel.Online)]
    [Arguments(LeadChannel.Offline)]
    [Arguments(LeadChannel.Referral)]
    [Arguments(LeadChannel.Direct)]
    [Arguments(LeadChannel.Other)]
    public void GivenAnyChannel_WhenCreating_ThenShouldBeStored(LeadChannel channel)
    {
        var source = new LeadSource(OrgId, "Источник", channel, null);

        source.Channel.ShouldBe(channel);
    }
}
