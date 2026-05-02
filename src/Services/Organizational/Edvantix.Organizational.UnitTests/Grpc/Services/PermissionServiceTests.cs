using Edvantix.Organizational.Grpc.Services;
using Edvantix.Organizational.Grpc.Services.Permissions;
using Edvantix.Organizational.UnitTests.Grpc.Context;

namespace Edvantix.Organizational.UnitTests.Grpc.Services;

public sealed class PermissionServiceTests
{
    private readonly Mock<IPermissionRepository> _repoMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    public PermissionServiceTests()
    {
        _repoMock.Setup(r => r.UnitOfWork).Returns(_unitOfWorkMock.Object);
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
    }

    private PermissionService CreateService() => new(_repoMock.Object);

    private static TestServerCallContext CreateContext() => new();

    // ─── CheckPermission ───────────────────────────────────────────────────────

    [Test]
    public async Task GivenCheckPermissionCalled_WhenHandled_ThenShouldThrowNotImplementedException()
    {
        var service = CreateService();
        var context = CreateContext();

        await Should.ThrowAsync<NotImplementedException>(() =>
            service.CheckPermission(new CheckPermissionRequest(), context)
        );
    }

    // ─── SyncFeaturePermissions – Guard validations ────────────────────────────
    // Note: proto3 string fields cannot be null (protobuf throws ArgumentNullException before service code).
    // We test empty and whitespace-only values, which reach Guard.Against inside the service.

    [Test]
    [Arguments("")]
    [Arguments("   ")]
    public async Task GivenEmptyOrWhiteSpaceServiceCode_WhenSyncFeaturePermissions_ThenShouldThrowArgumentException(
        string serviceCode
    )
    {
        var request = new SyncFeaturePermissionsRequest
        {
            ServiceCode = serviceCode,
            FeatureCode = "Organization",
            FeatureName = "Организация",
        };

        await Should.ThrowAsync<ArgumentException>(() =>
            CreateService().SyncFeaturePermissions(request, CreateContext())
        );
    }

    [Test]
    [Arguments("")]
    [Arguments("   ")]
    public async Task GivenEmptyOrWhiteSpaceFeatureCode_WhenSyncFeaturePermissions_ThenShouldThrowArgumentException(
        string featureCode
    )
    {
        var request = new SyncFeaturePermissionsRequest
        {
            ServiceCode = "organizational",
            FeatureCode = featureCode,
            FeatureName = "Организация",
        };

        await Should.ThrowAsync<ArgumentException>(() =>
            CreateService().SyncFeaturePermissions(request, CreateContext())
        );
    }

    [Test]
    [Arguments("")]
    [Arguments("   ")]
    public async Task GivenEmptyOrWhiteSpaceFeatureName_WhenSyncFeaturePermissions_ThenShouldThrowArgumentException(
        string featureName
    )
    {
        var request = new SyncFeaturePermissionsRequest
        {
            ServiceCode = "organizational",
            FeatureCode = "Organization",
            FeatureName = featureName,
        };

        await Should.ThrowAsync<ArgumentException>(() =>
            CreateService().SyncFeaturePermissions(request, CreateContext())
        );
    }

    // ─── SyncFeaturePermissions – Add new permissions ─────────────────────────

    [Test]
    public async Task GivenNoExistingPermissions_WhenSyncFeaturePermissionsWithNewEntries_ThenShouldAddAllAndReturnAddedCount()
    {
        _repoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync([]);

        var request = BuildRequest([
            new PermissionEntry { Code = "View", Name = "Просмотр" },
            new PermissionEntry { Code = "Edit", Name = "Редактирование" },
        ]);

        var response = await CreateService().SyncFeaturePermissions(request, CreateContext());

        response.Added.ShouldBe(2);
        response.Removed.ShouldBe(0);
        _repoMock.Verify(r => r.Add(It.IsAny<Permission>()), Times.Exactly(2));
    }

    // ─── SyncFeaturePermissions – Remove obsolete permissions ─────────────────

    [Test]
    public async Task GivenExistingPermissionsNotInRequest_WhenSyncFeaturePermissions_ThenShouldRemoveObsoleteAndReturnRemovedCount()
    {
        var existing = new List<Permission>
        {
            CreatePermission("View"),
            CreatePermission("Delete"),
        };
        _repoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(existing);

        var request = BuildRequest([new PermissionEntry { Code = "View", Name = "Просмотр" }]);

        var response = await CreateService().SyncFeaturePermissions(request, CreateContext());

        response.Added.ShouldBe(0);
        response.Removed.ShouldBe(1);
        _repoMock.Verify(r => r.Remove(It.Is<Permission>(p => p.Code == "Delete")), Times.Once);
        _repoMock.Verify(r => r.Remove(It.Is<Permission>(p => p.Code == "View")), Times.Never);
    }

    // ─── SyncFeaturePermissions – Update existing permissions ─────────────────

    [Test]
    public async Task GivenExistingPermissionsInRequest_WhenSyncFeaturePermissions_ThenShouldUpdateNamesAndReturnZeroCounts()
    {
        var existing = new List<Permission> { CreatePermission("View", "Старое название") };
        _repoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(existing);

        var request = BuildRequest(
            [new PermissionEntry { Code = "View", Name = "Новое название" }],
            featureName: "Новая область"
        );

        var response = await CreateService().SyncFeaturePermissions(request, CreateContext());

        response.Added.ShouldBe(0);
        response.Removed.ShouldBe(0);
        existing[0].FeatureName.ShouldBe("Новая область");
        existing[0].Name.ShouldBe("Новое название");
    }

    // ─── SyncFeaturePermissions – Mixed scenario ──────────────────────────────

    [Test]
    public async Task GivenMixedPermissions_WhenSyncFeaturePermissions_ThenShouldAddRemoveAndUpdateCorrectly()
    {
        var existing = new List<Permission>
        {
            CreatePermission("View"),
            CreatePermission("Delete"),
        };
        _repoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(existing);

        var request = BuildRequest([
            new PermissionEntry { Code = "View", Name = "Обновлено" },
            new PermissionEntry { Code = "Create", Name = "Создание" },
        ]);

        var response = await CreateService().SyncFeaturePermissions(request, CreateContext());

        response.Added.ShouldBe(1);
        response.Removed.ShouldBe(1);
        _repoMock.Verify(r => r.Add(It.Is<Permission>(p => p.Code == "Create")), Times.Once);
        _repoMock.Verify(r => r.Remove(It.Is<Permission>(p => p.Code == "Delete")), Times.Once);
    }

    // ─── SyncFeaturePermissions – Empty request removes all ───────────────────

    [Test]
    public async Task GivenEmptyPermissionsList_WhenSyncFeaturePermissions_ThenShouldRemoveAllExistingAndReturnRemovedCount()
    {
        var existing = new List<Permission> { CreatePermission("View"), CreatePermission("Edit") };
        _repoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(existing);

        var request = BuildRequest([]);

        var response = await CreateService().SyncFeaturePermissions(request, CreateContext());

        response.Added.ShouldBe(0);
        response.Removed.ShouldBe(2);
        _repoMock.Verify(r => r.Remove(It.IsAny<Permission>()), Times.Exactly(2));
    }

    // ─── SyncFeaturePermissions – Isolation by ServiceCode/FeatureCode ────────

    [Test]
    public async Task GivenPermissionsForDifferentServiceCode_WhenSyncFeaturePermissions_ThenShouldNotModifyOtherServices()
    {
        var otherService = CreatePermission("View", serviceCode: "scheduler");
        _repoMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([otherService]);

        var request = BuildRequest([]);

        var response = await CreateService().SyncFeaturePermissions(request, CreateContext());

        response.Removed.ShouldBe(0);
        _repoMock.Verify(r => r.Remove(It.IsAny<Permission>()), Times.Never);
    }

    [Test]
    public async Task GivenPermissionsForDifferentFeatureCode_WhenSyncFeaturePermissions_ThenShouldNotModifyOtherFeatures()
    {
        var otherFeature = CreatePermission("View", featureCode: "Member");
        _repoMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([otherFeature]);

        var request = BuildRequest([]);

        var response = await CreateService().SyncFeaturePermissions(request, CreateContext());

        response.Removed.ShouldBe(0);
        _repoMock.Verify(r => r.Remove(It.IsAny<Permission>()), Times.Never);
    }

    // ─── SyncFeaturePermissions – Case-insensitive matching ───────────────────

    [Test]
    public async Task GivenExistingPermissionWithUpperCaseCode_WhenSyncFeaturePermissionsWithLowerCase_ThenShouldUpdateInsteadOfAdd()
    {
        var existing = new List<Permission> { CreatePermission("VIEW") };
        _repoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(existing);

        var request = BuildRequest([new PermissionEntry { Code = "view", Name = "Просмотр" }]);

        var response = await CreateService().SyncFeaturePermissions(request, CreateContext());

        response.Added.ShouldBe(0);
        response.Removed.ShouldBe(0);
        _repoMock.Verify(r => r.Add(It.IsAny<Permission>()), Times.Never);
    }

    [Test]
    public async Task GivenServiceCodeWithDifferentCase_WhenSyncFeaturePermissions_ThenShouldMatchExistingCaseInsensitively()
    {
        var existing = new List<Permission>
        {
            CreatePermission("View", serviceCode: "ORGANIZATIONAL"),
        };
        _repoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(existing);

        var request = BuildRequest([new PermissionEntry { Code = "View", Name = "Просмотр" }]);

        var response = await CreateService().SyncFeaturePermissions(request, CreateContext());

        response.Added.ShouldBe(0);
        response.Removed.ShouldBe(0);
    }

    // ─── SyncFeaturePermissions – Skip blank codes in request ─────────────────

    [Test]
    public async Task GivenPermissionEntriesWithEmptyCode_WhenSyncFeaturePermissions_ThenShouldSkipBlankEntries()
    {
        _repoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync([]);

        var request = BuildRequest([
            new PermissionEntry { Code = "", Name = "Пустой" },
            new PermissionEntry { Code = "   ", Name = "Пробелы" },
            new PermissionEntry { Code = "View", Name = "Просмотр" },
        ]);

        var response = await CreateService().SyncFeaturePermissions(request, CreateContext());

        response.Added.ShouldBe(1);
        _repoMock.Verify(r => r.Add(It.Is<Permission>(p => p.Code == "View")), Times.Once);
    }

    // ─── SyncFeaturePermissions – Trim whitespace in codes and names ──────────

    [Test]
    public async Task GivenPermissionEntryWithWhitespacePaddedCode_WhenSyncFeaturePermissions_ThenShouldTrimCodeBeforeAdding()
    {
        _repoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync([]);

        var request = BuildRequest([
            new PermissionEntry { Code = "  View  ", Name = "  Просмотр  " },
        ]);

        var response = await CreateService().SyncFeaturePermissions(request, CreateContext());

        response.Added.ShouldBe(1);
        _repoMock.Verify(r => r.Add(It.Is<Permission>(p => p.Code == "View")), Times.Once);
    }

    // ─── SyncFeaturePermissions – SaveChanges always called ───────────────────

    [Test]
    public async Task GivenAnyValidRequest_WhenSyncFeaturePermissions_ThenShouldAlwaysCallSaveChanges()
    {
        _repoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync([]);

        await CreateService().SyncFeaturePermissions(BuildRequest([]), CreateContext());

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    // ─── Helpers ──────────────────────────────────────────────────────────────

    private static SyncFeaturePermissionsRequest BuildRequest(
        IEnumerable<PermissionEntry> entries,
        string serviceCode = "organizational",
        string featureCode = "Organization",
        string featureName = "Организация"
    )
    {
        var request = new SyncFeaturePermissionsRequest
        {
            ServiceCode = serviceCode,
            FeatureCode = featureCode,
            FeatureName = featureName,
        };
        request.Permissions.AddRange(entries);
        return request;
    }

    private static Permission CreatePermission(
        string code,
        string name = "Название",
        string serviceCode = "organizational",
        string featureCode = "Organization",
        string featureName = "Организация"
    ) => new(serviceCode, featureCode, featureName, code, name);
}
