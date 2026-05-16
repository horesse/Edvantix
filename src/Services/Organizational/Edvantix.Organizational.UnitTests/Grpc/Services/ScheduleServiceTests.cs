using Edvantix.Organizational.Grpc.Services.Schedules;
using Edvantix.Schedule.Grpc.Services;
using Grpc.Core;

namespace Edvantix.Organizational.UnitTests.Grpc.Services;

public sealed class ScheduleServiceTests
{
    private readonly Mock<ScheduleGrpcService.ScheduleGrpcServiceClient> _clientMock = new();

    private ScheduleService CreateService() => new(_clientMock.Object);

    /// <summary>
    /// Оба overload'а gRPC-клиента virtual. Код вызывает вариант
    /// (request, Metadata, DateTime?, CancellationToken), поэтому его и мокируем.
    /// </summary>
    private static AsyncUnaryCall<T> GrpcCall<T>(T response) =>
        new(
            Task.FromResult(response),
            Task.FromResult(new Metadata()),
            () => Status.DefaultSuccess,
            () => new Metadata(),
            () => { }
        );

    // ─── GetScheduleSummariesAsync ────────────────────────────────────────────

    [Test]
    public async Task GivenEmptyGroupIds_WhenGetScheduleSummaries_ThenReturnsEmptyDictionary()
    {
        _clientMock
            .Setup(c =>
                c.GetScheduleSummariesByGroupIdsAsync(
                    It.IsAny<GetScheduleSummariesRequest>(),
                    It.IsAny<Metadata>(),
                    It.IsAny<DateTime?>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Returns(GrpcCall(new GetScheduleSummariesResponse()));

        var result = await CreateService().GetScheduleSummariesAsync([], CancellationToken.None);

        result.ShouldBeEmpty();
    }

    [Test]
    public async Task GivenGroupIdsWithSummaries_WhenGetScheduleSummaries_ThenMapsToDictionary()
    {
        var groupId = Guid.CreateVersion7();
        var response = new GetScheduleSummariesResponse();
        response.Summaries.Add(
            new ScheduleSummary
            {
                GroupId = groupId.ToString(),
                SummaryText = "Пн / Ср · 18:00–19:30",
                LessonDurationMinutes = 90,
                NextLessonDate = "2025-09-01",
                LessonCountTotal = 30,
                LessonCountRemaining = 28,
            }
        );

        _clientMock
            .Setup(c =>
                c.GetScheduleSummariesByGroupIdsAsync(
                    It.IsAny<GetScheduleSummariesRequest>(),
                    It.IsAny<Metadata>(),
                    It.IsAny<DateTime?>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Returns(GrpcCall(response));

        var result = await CreateService()
            .GetScheduleSummariesAsync([groupId], CancellationToken.None);

        result.ShouldContainKey(groupId);
        var dto = result[groupId];
        dto.SummaryText.ShouldBe("Пн / Ср · 18:00–19:30");
        dto.LessonDurationMinutes.ShouldBe(90);
        dto.NextLessonDate.ShouldBe(new DateOnly(2025, 9, 1));
        dto.LessonCountTotal.ShouldBe(30);
        dto.LessonCountRemaining.ShouldBe(28);
    }

    [Test]
    public async Task GivenSummaryWithEmptyNextLessonDate_WhenGetScheduleSummaries_ThenNextLessonDateIsNull()
    {
        var groupId = Guid.CreateVersion7();
        var response = new GetScheduleSummariesResponse();
        response.Summaries.Add(
            new ScheduleSummary
            {
                GroupId = groupId.ToString(),
                SummaryText = string.Empty,
                NextLessonDate = string.Empty,
            }
        );

        _clientMock
            .Setup(c =>
                c.GetScheduleSummariesByGroupIdsAsync(
                    It.IsAny<GetScheduleSummariesRequest>(),
                    It.IsAny<Metadata>(),
                    It.IsAny<DateTime?>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Returns(GrpcCall(response));

        var result = await CreateService()
            .GetScheduleSummariesAsync([groupId], CancellationToken.None);

        result[groupId].NextLessonDate.ShouldBeNull();
    }

    [Test]
    public async Task GivenMultipleGroupIds_WhenGetScheduleSummaries_ThenSendsAllIdsInRequest()
    {
        var id1 = Guid.CreateVersion7();
        var id2 = Guid.CreateVersion7();
        GetScheduleSummariesRequest? capturedRequest = null;

        _clientMock
            .Setup(c =>
                c.GetScheduleSummariesByGroupIdsAsync(
                    It.IsAny<GetScheduleSummariesRequest>(),
                    It.IsAny<Metadata>(),
                    It.IsAny<DateTime?>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Callback<GetScheduleSummariesRequest, Metadata, DateTime?, CancellationToken>(
                (req, _, _, _) => capturedRequest = req
            )
            .Returns(GrpcCall(new GetScheduleSummariesResponse()));

        await CreateService().GetScheduleSummariesAsync([id1, id2], CancellationToken.None);

        capturedRequest.ShouldNotBeNull();
        capturedRequest!.GroupIds.ShouldContain(id1.ToString());
        capturedRequest.GroupIds.ShouldContain(id2.ToString());
    }

    // ─── GetScheduleByGroupIdAsync ────────────────────────────────────────────

    [Test]
    public async Task GivenScheduleNotFound_WhenGetScheduleByGroupId_ThenReturnsNull()
    {
        _clientMock
            .Setup(c =>
                c.GetScheduleByGroupIdAsync(
                    It.IsAny<GetScheduleByGroupIdRequest>(),
                    It.IsAny<Metadata>(),
                    It.IsAny<DateTime?>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Returns(GrpcCall(new GetScheduleByGroupIdResponse { Found = false }));

        var result = await CreateService()
            .GetScheduleByGroupIdAsync(Guid.CreateVersion7(), CancellationToken.None);

        result.ShouldBeNull();
    }

    [Test]
    public async Task GivenScheduleFoundWithRequiredFields_WhenGetScheduleByGroupId_ThenMapsDto()
    {
        var scheduleId = Guid.CreateVersion7();
        var detail = new ScheduleDetail
        {
            Id = scheduleId.ToString(),
            Recurrence = "Weekly",
            LessonDurationMinutes = 90,
            StartDate = "2025-09-01",
            EndMode = "Date",
            SkipHolidays = false,
            SummaryText = "Пн / Ср · 18:00–19:30",
        };

        _clientMock
            .Setup(c =>
                c.GetScheduleByGroupIdAsync(
                    It.IsAny<GetScheduleByGroupIdRequest>(),
                    It.IsAny<Metadata>(),
                    It.IsAny<DateTime?>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Returns(
                GrpcCall(new GetScheduleByGroupIdResponse { Found = true, Schedule = detail })
            );

        var result = await CreateService()
            .GetScheduleByGroupIdAsync(Guid.CreateVersion7(), CancellationToken.None);

        result.ShouldNotBeNull();
        result!.Id.ShouldBe(scheduleId);
        result.Recurrence.ShouldBe("Weekly");
        result.LessonDurationMinutes.ShouldBe((short)90);
        result.StartDate.ShouldBe(new DateOnly(2025, 9, 1));
        result.EndMode.ShouldBe("Date");
        result.SummaryText.ShouldBe("Пн / Ср · 18:00–19:30");
    }

    [Test]
    public async Task GivenScheduleFoundWithoutOptionalFields_WhenGetScheduleByGroupId_ThenNullableFieldsAreNull()
    {
        var detail = new ScheduleDetail
        {
            Id = Guid.CreateVersion7().ToString(),
            Recurrence = "Weekly",
            LessonDurationMinutes = 60,
            StartDate = "2025-09-01",
            EndMode = "Count",
            SkipHolidays = false,
            SummaryText = string.Empty,
        };

        _clientMock
            .Setup(c =>
                c.GetScheduleByGroupIdAsync(
                    It.IsAny<GetScheduleByGroupIdRequest>(),
                    It.IsAny<Metadata>(),
                    It.IsAny<DateTime?>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Returns(
                GrpcCall(new GetScheduleByGroupIdResponse { Found = true, Schedule = detail })
            );

        var result = await CreateService()
            .GetScheduleByGroupIdAsync(Guid.CreateVersion7(), CancellationToken.None);

        result.ShouldNotBeNull();
        result!.BiweeklyParity.ShouldBeNull();
        result.EndDate.ShouldBeNull();
        result.LessonCount.ShouldBeNull();
    }

    [Test]
    public async Task GivenScheduleFoundWithOptionalFields_WhenGetScheduleByGroupId_ThenNullableFieldsAreMapped()
    {
        var detail = new ScheduleDetail
        {
            Id = Guid.CreateVersion7().ToString(),
            Recurrence = "Biweekly",
            BiweeklyParity = 1,
            LessonDurationMinutes = 60,
            StartDate = "2025-09-01",
            EndMode = "Date",
            EndDate = "2026-06-30",
            LessonCount = 20,
            SkipHolidays = true,
            SummaryText = string.Empty,
        };

        _clientMock
            .Setup(c =>
                c.GetScheduleByGroupIdAsync(
                    It.IsAny<GetScheduleByGroupIdRequest>(),
                    It.IsAny<Metadata>(),
                    It.IsAny<DateTime?>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Returns(
                GrpcCall(new GetScheduleByGroupIdResponse { Found = true, Schedule = detail })
            );

        var result = await CreateService()
            .GetScheduleByGroupIdAsync(Guid.CreateVersion7(), CancellationToken.None);

        result.ShouldNotBeNull();
        result!.BiweeklyParity.ShouldBe(1);
        result.EndDate.ShouldBe(new DateOnly(2026, 6, 30));
        result.LessonCount.ShouldBe((short)20);
        result.SkipHolidays.ShouldBeTrue();
    }

    [Test]
    public async Task GivenScheduleFoundWithSlotsAndExceptions_WhenGetScheduleByGroupId_ThenCollectionsMapped()
    {
        var detail = new ScheduleDetail
        {
            Id = Guid.CreateVersion7().ToString(),
            Recurrence = "Weekly",
            LessonDurationMinutes = 90,
            StartDate = "2025-09-01",
            EndMode = "Date",
            SummaryText = string.Empty,
        };
        detail.Slots.Add(new ScheduleSlotProto { Weekday = 1, StartMinutes = 1080 });
        detail.Slots.Add(new ScheduleSlotProto { Weekday = 3, StartMinutes = 1080 });
        detail.Exceptions.Add(
            new ScheduleExceptionProto { Date = "2025-11-04", Reason = "Праздник" }
        );

        _clientMock
            .Setup(c =>
                c.GetScheduleByGroupIdAsync(
                    It.IsAny<GetScheduleByGroupIdRequest>(),
                    It.IsAny<Metadata>(),
                    It.IsAny<DateTime?>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Returns(
                GrpcCall(new GetScheduleByGroupIdResponse { Found = true, Schedule = detail })
            );

        var result = await CreateService()
            .GetScheduleByGroupIdAsync(Guid.CreateVersion7(), CancellationToken.None);

        result.ShouldNotBeNull();
        result!.Slots.Count.ShouldBe(2);
        result.Slots[0].Weekday.ShouldBe(1);
        result.Slots[0].StartMinutes.ShouldBe(1080);
        result.Exceptions.Count.ShouldBe(1);
        result.Exceptions[0].Date.ShouldBe(new DateOnly(2025, 11, 4));
        result.Exceptions[0].Reason.ShouldBe("Праздник");
    }

    [Test]
    public async Task GivenExceptionWithEmptyReason_WhenGetScheduleByGroupId_ThenReasonIsNull()
    {
        var detail = new ScheduleDetail
        {
            Id = Guid.CreateVersion7().ToString(),
            Recurrence = "Weekly",
            LessonDurationMinutes = 60,
            StartDate = "2025-09-01",
            EndMode = "Date",
            SummaryText = string.Empty,
        };
        detail.Exceptions.Add(
            new ScheduleExceptionProto { Date = "2025-11-04", Reason = string.Empty }
        );

        _clientMock
            .Setup(c =>
                c.GetScheduleByGroupIdAsync(
                    It.IsAny<GetScheduleByGroupIdRequest>(),
                    It.IsAny<Metadata>(),
                    It.IsAny<DateTime?>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Returns(
                GrpcCall(new GetScheduleByGroupIdResponse { Found = true, Schedule = detail })
            );

        var result = await CreateService()
            .GetScheduleByGroupIdAsync(Guid.CreateVersion7(), CancellationToken.None);

        result!.Exceptions[0].Reason.ShouldBeNull();
    }

    // ─── GetUpcomingLessonsAsync ──────────────────────────────────────────────

    [Test]
    public async Task GivenNoUpcomingLessons_WhenGetUpcomingLessons_ThenReturnsEmptyList()
    {
        _clientMock
            .Setup(c =>
                c.GetUpcomingLessonsAsync(
                    It.IsAny<GetUpcomingLessonsRequest>(),
                    It.IsAny<Metadata>(),
                    It.IsAny<DateTime?>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Returns(GrpcCall(new GetUpcomingLessonsResponse()));

        var result = await CreateService()
            .GetUpcomingLessonsAsync(Guid.CreateVersion7(), 5, CancellationToken.None);

        result.ShouldBeEmpty();
    }

    [Test]
    public async Task GivenUpcomingLessons_WhenGetUpcomingLessons_ThenMapsDateAndTimeCorrectly()
    {
        var response = new GetUpcomingLessonsResponse();
        response.Lessons.Add(
            new UpcomingLessonProto
            {
                Date = "2025-09-01",
                StartMinutes = 1080, // 18:00
                DurationMinutes = 90,
            }
        );

        _clientMock
            .Setup(c =>
                c.GetUpcomingLessonsAsync(
                    It.IsAny<GetUpcomingLessonsRequest>(),
                    It.IsAny<Metadata>(),
                    It.IsAny<DateTime?>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Returns(GrpcCall(response));

        var result = await CreateService()
            .GetUpcomingLessonsAsync(Guid.CreateVersion7(), 5, CancellationToken.None);

        result.Count.ShouldBe(1);
        result[0].Date.ShouldBe(new DateOnly(2025, 9, 1));
        result[0].StartTime.ShouldBe(new TimeOnly(18, 0));
        result[0].EndTime.ShouldBe(new TimeOnly(19, 30));
    }

    [Test]
    public async Task GivenCountParameter_WhenGetUpcomingLessons_ThenPassesCountToRequest()
    {
        GetUpcomingLessonsRequest? capturedRequest = null;

        _clientMock
            .Setup(c =>
                c.GetUpcomingLessonsAsync(
                    It.IsAny<GetUpcomingLessonsRequest>(),
                    It.IsAny<Metadata>(),
                    It.IsAny<DateTime?>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Callback<GetUpcomingLessonsRequest, Metadata, DateTime?, CancellationToken>(
                (req, _, _, _) => capturedRequest = req
            )
            .Returns(GrpcCall(new GetUpcomingLessonsResponse()));

        var groupId = Guid.CreateVersion7();
        await CreateService().GetUpcomingLessonsAsync(groupId, count: 3, CancellationToken.None);

        capturedRequest.ShouldNotBeNull();
        capturedRequest!.GroupId.ShouldBe(groupId.ToString());
        capturedRequest.Count.ShouldBe(3);
    }

    [Test]
    public async Task GivenMultipleLessons_WhenGetUpcomingLessons_ThenAllLessonsAreMapped()
    {
        var response = new GetUpcomingLessonsResponse();
        response.Lessons.Add(
            new UpcomingLessonProto { Date = "2025-09-01", StartMinutes = 1080, DurationMinutes = 90 }
        );
        response.Lessons.Add(
            new UpcomingLessonProto { Date = "2025-09-03", StartMinutes = 1080, DurationMinutes = 90 }
        );
        response.Lessons.Add(
            new UpcomingLessonProto { Date = "2025-09-08", StartMinutes = 1080, DurationMinutes = 90 }
        );

        _clientMock
            .Setup(c =>
                c.GetUpcomingLessonsAsync(
                    It.IsAny<GetUpcomingLessonsRequest>(),
                    It.IsAny<Metadata>(),
                    It.IsAny<DateTime?>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Returns(GrpcCall(response));

        var result = await CreateService()
            .GetUpcomingLessonsAsync(Guid.CreateVersion7(), 5, CancellationToken.None);

        result.Count.ShouldBe(3);
        result[1].Date.ShouldBe(new DateOnly(2025, 9, 3));
        result[2].Date.ShouldBe(new DateOnly(2025, 9, 8));
    }
}
