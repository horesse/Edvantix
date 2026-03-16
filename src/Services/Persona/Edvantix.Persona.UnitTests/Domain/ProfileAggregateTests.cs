namespace Edvantix.Persona.UnitTests.Domain;

public sealed class ProfileAggregateTests
{
    private static Profile CreateValidProfile() =>
        new(
            Guid.CreateVersion7(),
            "testuser",
            Gender.Male,
            new DateOnly(1990, 6, 15),
            "Иван",
            "Иванов"
        );

    [Test]
    public void GivenValidData_WhenCreatingProfile_ThenShouldInitializePropertiesCorrectly()
    {
        var accountId = Guid.CreateVersion7();
        const string login = "testuser";
        var birthDate = new DateOnly(1990, 6, 15);

        var profile = new Profile(
            accountId,
            login,
            Gender.Female,
            birthDate,
            "Анна",
            "Петрова",
            "Ивановна"
        );

        profile.AccountId.ShouldBe(accountId);
        profile.Login.ShouldBe(login);
        profile.Gender.ShouldBe(Gender.Female);
        profile.BirthDate.ShouldBe(birthDate);
        profile.FullName.FirstName.ShouldBe("Анна");
        profile.FullName.LastName.ShouldBe("Петрова");
        profile.FullName.MiddleName.ShouldBe("Ивановна");
        profile.AvatarUrl.ShouldBeNull();
        profile.Bio.ShouldBeNull();
        profile.IsDeleted.ShouldBeFalse();
        profile.Contacts.ShouldBeEmpty();
        profile.Educations.ShouldBeEmpty();
        profile.EmploymentHistories.ShouldBeEmpty();
        profile.Skills.ShouldBeEmpty();
    }

    [Test]
    public void GivenValidData_WhenCreatingProfile_ThenShouldRegisterProfileRegisteredEvent()
    {
        var accountId = Guid.CreateVersion7();
        const string login = "john.doe";

        var profile = new Profile(
            accountId,
            login,
            Gender.Male,
            new DateOnly(1990, 1, 1),
            "Иван",
            "Иванов"
        );

        profile.DomainEvents.ShouldHaveSingleItem();
        var @event = profile.DomainEvents.Single().ShouldBeOfType<ProfileRegisteredEvent>();
        @event.AccountId.ShouldBe(accountId);
        @event.Login.ShouldBe(login);
    }

    [Test]
    public void GivenEmptyAccountId_WhenCreatingProfile_ThenShouldThrowArgumentException()
    {
        var act = () =>
            new Profile(
                Guid.Empty,
                "testuser",
                Gender.Male,
                new DateOnly(1990, 1, 1),
                "Иван",
                "Иванов"
            );

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("   ")]
    public void GivenEmptyOrNullLogin_WhenCreatingProfile_ThenShouldThrowArgumentException(
        string? login
    )
    {
        var act = () =>
            new Profile(
                Guid.CreateVersion7(),
                login!,
                Gender.Male,
                new DateOnly(1990, 1, 1),
                "Иван",
                "Иванов"
            );

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenNewBirthDate_WhenUpdatingPersonalInfo_ThenShouldUpdateBirthDate()
    {
        var profile = CreateValidProfile();
        var newBirthDate = new DateOnly(1985, 3, 25);

        profile.UpdatePersonalInfo(newBirthDate);

        profile.BirthDate.ShouldBe(newBirthDate);
    }

    [Test]
    public void GivenValidNames_WhenUpdatingFullName_ThenShouldUpdateAllFields()
    {
        var profile = CreateValidProfile();

        profile.UpdateFullName("Пётр", "Петров", "Иванович");

        profile.FullName.FirstName.ShouldBe("Пётр");
        profile.FullName.LastName.ShouldBe("Петров");
        profile.FullName.MiddleName.ShouldBe("Иванович");
    }

    [Test]
    public void GivenNullMiddleName_WhenUpdatingFullName_ThenShouldClearMiddleName()
    {
        var profile = new Profile(
            Guid.CreateVersion7(),
            "testuser",
            Gender.Male,
            new DateOnly(1990, 1, 1),
            "Иван",
            "Иванов",
            "Петрович"
        );

        profile.UpdateFullName("Иван", "Иванов");

        profile.FullName.MiddleName.ShouldBeNull();
    }

    [Test]
    public void GivenNoExistingAvatar_WhenUploadingAvatar_ThenShouldSetAvatarUrlWithoutEvent()
    {
        var profile = CreateValidProfile();
        profile.ClearDomainEvents();
        const string avatarUrn = "urn:blob:avatars/photo.jpg";

        profile.UploadAvatar(avatarUrn);

        profile.AvatarUrl.ShouldBe(avatarUrn);
        profile.DomainEvents.ShouldBeEmpty();
    }

    [Test]
    public void GivenExistingAvatar_WhenUploadingNewAvatar_ThenShouldRegisterAvatarDeletedEventForOldUrl()
    {
        var profile = CreateValidProfile();
        const string oldAvatarUrn = "urn:blob:avatars/old.jpg";
        const string newAvatarUrn = "urn:blob:avatars/new.jpg";
        profile.UploadAvatar(oldAvatarUrn);
        profile.ClearDomainEvents();

        profile.UploadAvatar(newAvatarUrn);

        profile.AvatarUrl.ShouldBe(newAvatarUrn);
        profile.DomainEvents.ShouldHaveSingleItem();
        var @event = profile.DomainEvents.Single().ShouldBeOfType<AvatarDeletedDomainEvent>();
        @event.AvatarUrn.ShouldBe(oldAvatarUrn);
    }

    [Test]
    public void GivenExistingAvatar_WhenDeletingAvatar_ThenShouldClearAvatarUrlAndRegisterEvent()
    {
        var profile = CreateValidProfile();
        const string avatarUrn = "urn:blob:avatars/photo.jpg";
        profile.UploadAvatar(avatarUrn);
        profile.ClearDomainEvents();

        profile.DeleteAvatar();

        profile.AvatarUrl.ShouldBeNull();
        profile.DomainEvents.ShouldHaveSingleItem();
        var @event = profile.DomainEvents.Single().ShouldBeOfType<AvatarDeletedDomainEvent>();
        @event.AvatarUrn.ShouldBe(avatarUrn);
    }

    [Test]
    public void GivenNoAvatar_WhenDeletingAvatar_ThenShouldNotRegisterAnyEvent()
    {
        var profile = CreateValidProfile();
        profile.ClearDomainEvents();

        profile.DeleteAvatar();

        profile.AvatarUrl.ShouldBeNull();
        profile.DomainEvents.ShouldBeEmpty();
    }

    [Test]
    public void GivenBioText_WhenUpdatingBio_ThenShouldSetBio()
    {
        var profile = CreateValidProfile();
        const string bio = "Опытный .NET разработчик с 10 годами опыта.";

        profile.UpdateBio(bio);

        profile.Bio.ShouldBe(bio);
    }

    [Test]
    public void GivenNullBio_WhenUpdatingBio_ThenShouldClearBio()
    {
        var profile = CreateValidProfile();
        profile.UpdateBio("первоначальное описание");

        profile.UpdateBio(null);

        profile.Bio.ShouldBeNull();
    }

    [Test]
    public void GivenNewContacts_WhenReplacingContacts_ThenShouldContainNewContacts()
    {
        var profile = CreateValidProfile();
        var contacts = new[]
        {
            profile.CreateContact(ContactType.Email, "test@example.com"),
            profile.CreateContact(ContactType.Phone, "+79001234567", "рабочий"),
        };

        profile.ReplaceContacts(contacts);

        profile.Contacts.Count.ShouldBe(2);
    }

    [Test]
    public void GivenExistingContacts_WhenReplacingWithEmptyList_ThenShouldClearContacts()
    {
        var profile = CreateValidProfile();
        profile.ReplaceContacts([profile.CreateContact(ContactType.Email, "old@example.com")]);

        profile.ReplaceContacts([]);

        profile.Contacts.ShouldBeEmpty();
    }

    [Test]
    public void GivenValidSkillIds_WhenReplacingSkills_ThenShouldContainNewSkills()
    {
        var profile = CreateValidProfile();
        var skillIds = new[] { Guid.CreateVersion7(), Guid.CreateVersion7() };

        profile.ReplaceSkills(skillIds);

        profile.Skills.Count.ShouldBe(2);
        profile.Skills.Select(s => s.SkillId).ShouldBe(skillIds, ignoreOrder: true);
    }

    [Test]
    public void GivenMoreThanMaxSkills_WhenReplacingSkills_ThenShouldThrowInvalidOperationException()
    {
        var profile = CreateValidProfile();
        var tooManySkillIds = Enumerable
            .Range(0, Profile.MaxSkillsCount + 1)
            .Select(_ => Guid.CreateVersion7())
            .ToList();

        var act = () => profile.ReplaceSkills(tooManySkillIds);

        act.ShouldThrow<InvalidOperationException>();
    }

    [Test]
    public void GivenExistingSkills_WhenReplacingWithFewerSkills_ThenShouldPublishRemovedEventsForEachRemovedSkill()
    {
        var profile = CreateValidProfile();
        profile.Id = Guid.CreateVersion7();
        var keepSkillId = Guid.CreateVersion7();
        var removeSkillId1 = Guid.CreateVersion7();
        var removeSkillId2 = Guid.CreateVersion7();
        profile.ReplaceSkills([keepSkillId, removeSkillId1, removeSkillId2]);
        profile.ClearDomainEvents();

        profile.ReplaceSkills([keepSkillId]);

        var removedEvents = profile
            .DomainEvents.OfType<SkillRemovedFromProfileDomainEvent>()
            .ToList();
        removedEvents.Count.ShouldBe(2);
        removedEvents
            .Select(e => e.SkillId)
            .ShouldBe([removeSkillId1, removeSkillId2], ignoreOrder: true);
        removedEvents.ShouldAllBe(e => e.ProfileId == profile.Id);
    }

    [Test]
    public void GivenActiveProfile_WhenDeleting_ThenShouldMarkAsDeleted()
    {
        var profile = CreateValidProfile();

        profile.Delete();

        profile.IsDeleted.ShouldBeTrue();
    }
}
