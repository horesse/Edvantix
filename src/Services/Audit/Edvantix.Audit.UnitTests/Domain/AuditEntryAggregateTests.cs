namespace Edvantix.Audit.UnitTests.Domain;

public sealed class AuditEntryAggregateTests
{
    private static AuditEntry CreateValidEntry() =>
        new(
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            AuditAction.Created,
            AuditEntityType.Organization
        );

    [Test]
    public void GivenValidData_WhenCreatingAuditEntry_ThenShouldInitializeAllPropertiesCorrectly()
    {
        var organizationId = Guid.CreateVersion7();
        var actorId = Guid.CreateVersion7();
        var entityId = Guid.CreateVersion7();
        const string description = "Создана новая организация";
        const string metadata = "{\"name\":\"Acme\"}";
        const string ipAddress = "192.168.1.1";
        const string userAgent = "Mozilla/5.0";

        var entry = new AuditEntry(
            organizationId,
            actorId,
            AuditAction.Created,
            AuditEntityType.Organization,
            entityId,
            description,
            metadata,
            ipAddress,
            userAgent
        );

        entry.OrganizationId.ShouldBe(organizationId);
        entry.ActorId.ShouldBe(actorId);
        entry.Action.ShouldBe(AuditAction.Created);
        entry.EntityType.ShouldBe(AuditEntityType.Organization);
        entry.EntityId.ShouldBe(entityId);
        entry.Description.ShouldBe(description);
        entry.Metadata.ShouldBe(metadata);
        entry.IpAddress.ShouldBe(ipAddress);
        entry.UserAgent.ShouldBe(userAgent);
    }

    [Test]
    public void GivenRequiredFieldsOnly_WhenCreatingAuditEntry_ThenOptionalFieldsShouldBeNull()
    {
        var entry = CreateValidEntry();

        entry.EntityId.ShouldBeNull();
        entry.Description.ShouldBeNull();
        entry.Metadata.ShouldBeNull();
        entry.IpAddress.ShouldBeNull();
        entry.UserAgent.ShouldBeNull();
    }

    [Test]
    public void GivenValidData_WhenCreatingAuditEntry_ThenIdShouldNotBeEmpty()
    {
        var entry = CreateValidEntry();

        entry.Id.ShouldNotBe(Guid.Empty);
    }

    [Test]
    public void GivenValidData_WhenCreatingAuditEntry_ThenOccurredAtShouldBeSet()
    {
        var entry = CreateValidEntry();

        entry.OccurredAt.ShouldNotBe(default);
    }

    [Test]
    public void GivenValidData_WhenCreatingAuditEntry_ThenShouldRegisterSingleDomainEvent()
    {
        var entry = CreateValidEntry();

        entry.DomainEvents.ShouldHaveSingleItem();
    }

    [Test]
    public void GivenValidData_WhenCreatingAuditEntry_ThenDomainEventShouldContainCorrectData()
    {
        var organizationId = Guid.CreateVersion7();
        var actorId = Guid.CreateVersion7();

        var entry = new AuditEntry(
            organizationId,
            actorId,
            AuditAction.Updated,
            AuditEntityType.OrganizationMember
        );

        var @event = entry.DomainEvents.Single().ShouldBeOfType<AuditEntryCreatedDomainEvent>();
        @event.AuditEntryId.ShouldBe(entry.Id);
        @event.OrganizationId.ShouldBe(organizationId);
        @event.ActorId.ShouldBe(actorId);
        @event.Action.ShouldBe(AuditAction.Updated);
        @event.EntityType.ShouldBe(AuditEntityType.OrganizationMember);
    }

    [Test]
    public void GivenEmptyOrganizationId_WhenCreatingAuditEntry_ThenShouldThrowArgumentException()
    {
        var act = () =>
            new AuditEntry(
                Guid.Empty,
                Guid.CreateVersion7(),
                AuditAction.Created,
                AuditEntityType.Organization
            );

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenEmptyActorId_WhenCreatingAuditEntry_ThenShouldThrowArgumentException()
    {
        var act = () =>
            new AuditEntry(
                Guid.CreateVersion7(),
                Guid.Empty,
                AuditAction.Created,
                AuditEntityType.Organization
            );

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenDescriptionWithLeadingAndTrailingWhitespace_WhenCreatingAuditEntry_ThenDescriptionShouldBeTrimmed()
    {
        var entry = new AuditEntry(
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            AuditAction.Created,
            AuditEntityType.Organization,
            description: "  Создана организация  "
        );

        entry.Description.ShouldBe("Создана организация");
    }

    [Test]
    public void GivenIpAddressWithWhitespace_WhenCreatingAuditEntry_ThenIpAddressShouldBeTrimmed()
    {
        var entry = new AuditEntry(
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            AuditAction.Created,
            AuditEntityType.Organization,
            ipAddress: "  10.0.0.1  "
        );

        entry.IpAddress.ShouldBe("10.0.0.1");
    }

    [Test]
    public void GivenUserAgentWithWhitespace_WhenCreatingAuditEntry_ThenUserAgentShouldBeTrimmed()
    {
        var entry = new AuditEntry(
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            AuditAction.Created,
            AuditEntityType.Organization,
            userAgent: "  Mozilla/5.0 (Windows NT 10.0)  "
        );

        entry.UserAgent.ShouldBe("Mozilla/5.0 (Windows NT 10.0)");
    }

    [Test]
    public void GivenNullDescription_WhenCreatingAuditEntry_ThenDescriptionShouldBeNull()
    {
        var entry = new AuditEntry(
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            AuditAction.Created,
            AuditEntityType.Organization,
            description: null
        );

        entry.Description.ShouldBeNull();
    }

    [Test]
    public void GivenMetadataJson_WhenCreatingAuditEntry_ThenMetadataShouldBeStoredAsIs()
    {
        const string metadata = "{\"previousName\":\"Old Corp\",\"newName\":\"New Corp\"}";

        var entry = new AuditEntry(
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            AuditAction.Updated,
            AuditEntityType.Organization,
            metadata: metadata
        );

        entry.Metadata.ShouldBe(metadata);
    }

    [Test]
    public void GivenEntityId_WhenCreatingAuditEntry_ThenEntityIdShouldBeStored()
    {
        var entityId = Guid.CreateVersion7();

        var entry = new AuditEntry(
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            AuditAction.Deleted,
            AuditEntityType.OrganizationMember,
            entityId: entityId
        );

        entry.EntityId.ShouldBe(entityId);
    }

    [Test]
    [Arguments(AuditAction.Created)]
    [Arguments(AuditAction.Updated)]
    [Arguments(AuditAction.Deleted)]
    [Arguments(AuditAction.Archived)]
    [Arguments(AuditAction.Restored)]
    [Arguments(AuditAction.InvitationSent)]
    [Arguments(AuditAction.InvitationAccepted)]
    [Arguments(AuditAction.InvitationDeclined)]
    [Arguments(AuditAction.InvitationRevoked)]
    [Arguments(AuditAction.RoleAssigned)]
    [Arguments(AuditAction.RoleChanged)]
    [Arguments(AuditAction.PermissionGranted)]
    [Arguments(AuditAction.PermissionRevoked)]
    [Arguments(AuditAction.StatusChanged)]
    public void GivenAuditAction_WhenCreatingAuditEntry_ThenActionShouldBeStored(AuditAction action)
    {
        var entry = new AuditEntry(
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            action,
            AuditEntityType.Organization
        );

        entry.Action.ShouldBe(action);
    }

    [Test]
    [Arguments(AuditEntityType.Organization)]
    [Arguments(AuditEntityType.OrganizationMember)]
    [Arguments(AuditEntityType.Group)]
    [Arguments(AuditEntityType.GroupMember)]
    [Arguments(AuditEntityType.Invitation)]
    [Arguments(AuditEntityType.Role)]
    [Arguments(AuditEntityType.Permission)]
    [Arguments(AuditEntityType.Profile)]
    public void GivenEntityType_WhenCreatingAuditEntry_ThenEntityTypeShouldBeStored(
        AuditEntityType entityType
    )
    {
        var entry = new AuditEntry(
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            AuditAction.Created,
            entityType
        );

        entry.EntityType.ShouldBe(entityType);
    }
}
