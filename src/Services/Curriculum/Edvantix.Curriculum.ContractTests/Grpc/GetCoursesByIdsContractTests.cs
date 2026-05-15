using Edvantix.Common;

namespace Edvantix.Curriculum.ContractTests.Grpc;

/// <summary>
/// Контрактные тесты для gRPC-сообщений GetCoursesByIds.
/// Проверяют стабильность структуры proto-сообщений через snapshot-тестирование.
/// </summary>
public sealed class GetCoursesByIdsContractTests
{
    [Test]
    public async Task GivenGetCoursesByIdsRequest_WhenSerialized_ThenMatchesSnapshot()
    {
        var request = new GetCoursesByIdsRequest();
        request.CourseIds.Add("11111111-1111-7111-8111-111111111111");
        request.CourseIds.Add("22222222-2222-7222-8222-222222222222");

        await SnapshotTestHelper.Verify(request);
    }

    [Test]
    public async Task GivenGetCoursesByIdsResponse_WhenSerialized_ThenMatchesSnapshot()
    {
        var response = new GetCoursesByIdsResponse();
        response.Courses.Add(new CourseRef
        {
            Id = "11111111-1111-7111-8111-111111111111",
            Code = "EN-GEN-B1",
            Name = "English General B1",
        });
        response.Courses.Add(new CourseRef
        {
            Id = "22222222-2222-7222-8222-222222222222",
            Code = "EN-GEN-B2",
            Name = "English General B2",
        });

        await SnapshotTestHelper.Verify(response);
    }
}
