using Edvantix.Organizational.Features.Settings.Directories;

namespace Edvantix.Organizational.UnitTests.Features.Settings.Directories;

public sealed class DirectoryCatalogTests
{
    [Test]
    public void DirectoryCatalogAll_ShouldContainExactly8Items()
    {
        DirectoryCatalog.All.Count.ShouldBe(8);
    }

    [Test]
    public void DirectoryCatalogAll_ShouldBeInExpectedOrder()
    {
        string[] expected =
        [
            DirectoryCatalog.Levels,
            DirectoryCatalog.Subjects,
            DirectoryCatalog.LessonTypes,
            DirectoryCatalog.StudentStatuses,
            DirectoryCatalog.Rooms,
            DirectoryCatalog.Sources,
            DirectoryCatalog.PaymentMethods,
            DirectoryCatalog.Tags,
        ];

        DirectoryCatalog.All.Select(d => d.Code).ShouldBe(expected);
    }

    [Test]
    public void DirectoryCatalogAll_ShouldHaveUniqueCodes()
    {
        DirectoryCatalog.All.Select(d => d.Code).Distinct(StringComparer.Ordinal).Count().ShouldBe(8);
    }

    [Test]
    public void DirectoryCatalogAll_ShouldHaveNonEmptyNameAndIcon()
    {
        foreach (var descriptor in DirectoryCatalog.All)
        {
            descriptor.Name.ShouldNotBeNullOrWhiteSpace();
            descriptor.Description.ShouldNotBeNullOrWhiteSpace();
            descriptor.Icon.ShouldNotBeNullOrWhiteSpace();
        }
    }

    [Test]
    public void DirectoryCatalogFindByCode_KnownCode_ShouldReturnDescriptor()
    {
        var descriptor = DirectoryCatalog.FindByCode(DirectoryCatalog.StudentStatuses);

        descriptor.ShouldNotBeNull();
        descriptor!.Code.ShouldBe(DirectoryCatalog.StudentStatuses);
        descriptor.Badge.ShouldBe("системный");
    }

    [Test]
    public void DirectoryCatalogFindByCode_UnknownCode_ShouldReturnNull()
    {
        var descriptor = DirectoryCatalog.FindByCode("unknown-directory");

        descriptor.ShouldBeNull();
    }
}
