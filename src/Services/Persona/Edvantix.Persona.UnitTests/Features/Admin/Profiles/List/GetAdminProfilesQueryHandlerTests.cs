using Edvantix.Chassis.Specification;
using Edvantix.Persona.Features.Admin.Profiles;
using Edvantix.Persona.Features.Admin.Profiles.List;

namespace Edvantix.Persona.UnitTests.Features.Admin.Profiles.List;

public sealed class GetAdminProfilesQueryHandlerTests
{
    private readonly Mock<IProfileRepository> _profileRepoMock = new();
    private readonly Mock<IMapper<Profile, AdminProfileDto>> _mapperMock = new();
    private readonly GetAdminProfilesQueryHandler _handler;

    public GetAdminProfilesQueryHandlerTests()
    {
        _handler = new(_profileRepoMock.Object, _mapperMock.Object);
    }

    [Test]
    public async Task GivenProfilesExist_WhenHandling_ThenShouldReturnPagedResult()
    {
        var profile = CreateProfile();
        var dto = BuildDto(profile);
        var query = new GetAdminProfilesQuery(PageIndex: 1, PageSize: 10);

        _profileRepoMock
            .Setup(r =>
                r.FindAllAsync(It.IsAny<ISpecification<Profile>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync([profile]);
        _profileRepoMock
            .Setup(r =>
                r.CountAsync(It.IsAny<ISpecification<Profile>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(1);
        _mapperMock.Setup(m => m.Map(profile)).Returns(dto);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Count.ShouldBe(1);
        result.TotalItems.ShouldBe(1);
        result.PageIndex.ShouldBe(1);
        result.PageSize.ShouldBe(10);
    }

    [Test]
    public async Task GivenPageIndexBelowOne_WhenHandling_ThenShouldClampToOne()
    {
        var query = new GetAdminProfilesQuery(PageIndex: -5, PageSize: 10);

        _profileRepoMock
            .Setup(r =>
                r.FindAllAsync(It.IsAny<ISpecification<Profile>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync([]);
        _profileRepoMock
            .Setup(r =>
                r.CountAsync(It.IsAny<ISpecification<Profile>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(0);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.PageIndex.ShouldBe(1);
    }

    [Test]
    public async Task GivenPageSizeAbove100_WhenHandling_ThenShouldClampTo100()
    {
        var query = new GetAdminProfilesQuery(PageIndex: 1, PageSize: 500);

        _profileRepoMock
            .Setup(r =>
                r.FindAllAsync(It.IsAny<ISpecification<Profile>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync([]);
        _profileRepoMock
            .Setup(r =>
                r.CountAsync(It.IsAny<ISpecification<Profile>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(0);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.PageSize.ShouldBe(100);
    }

    [Test]
    public async Task GivenNoProfiles_WhenHandling_ThenShouldReturnEmptyPagedResult()
    {
        var query = new GetAdminProfilesQuery();

        _profileRepoMock
            .Setup(r =>
                r.FindAllAsync(It.IsAny<ISpecification<Profile>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync([]);
        _profileRepoMock
            .Setup(r =>
                r.CountAsync(It.IsAny<ISpecification<Profile>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(0);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.ShouldBeEmpty();
        result.TotalItems.ShouldBe(0);
    }

    private static Profile CreateProfile()
    {
        var profile = new Profile(
            Guid.CreateVersion7(),
            "testuser",
            Gender.Male,
            new DateOnly(1990, 1, 1),
            "Иван",
            "Иванов"
        )
        {
            Id = Guid.CreateVersion7(),
        };

        return profile;
    }

    private static AdminProfileDto BuildDto(Profile p) =>
        new(p.Id, p.AccountId, "Иванов Иван", "testuser", null, false, null);
}
