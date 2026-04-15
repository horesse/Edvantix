using Edvantix.Organizational.Domain.AggregatesModel.GroupMembershipHistoryAggregate;

namespace Edvantix.Organizational.UnitTests.Domain;

public sealed class GroupMembershipHistoryTests
{
    private static readonly Guid ValidMemberId = Guid.CreateVersion7();
    private static readonly DateOnly ValidJoinDate = new(2026, 1, 1);

    [Test]
    public void GivenValidData_WhenCreatingGroupMembershipHistory_ThenShouldInitializePropertiesCorrectly()
    {
        var history = new GroupMembershipHistory(ValidMemberId, ValidJoinDate);

        history.GroupMemberId.ShouldBe(ValidMemberId);
        history.JoinedAt.ShouldBe(ValidJoinDate);
        history.ExitedAt.ShouldBeNull();
        history.ExitReason.ShouldBeNull();
    }

    [Test]
    public void GivenEmptyGroupMemberId_WhenCreatingGroupMembershipHistory_ThenShouldThrowArgumentException()
    {
        var act = () => new GroupMembershipHistory(Guid.Empty, ValidJoinDate);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenValidExitDate_WhenRecordingExit_ThenShouldSetExitedAtAndExitReason()
    {
        var history = new GroupMembershipHistory(ValidMemberId, ValidJoinDate);
        var exitDate = ValidJoinDate.AddMonths(6);

        history.RecordExit(exitDate, "Отчисление");

        history.ExitedAt.ShouldBe(exitDate);
        history.ExitReason.ShouldBe("Отчисление");
    }

    [Test]
    public void GivenNullExitReason_WhenRecordingExit_ThenExitReasonShouldBeNull()
    {
        var history = new GroupMembershipHistory(ValidMemberId, ValidJoinDate);

        history.RecordExit(ValidJoinDate.AddDays(1), null);

        history.ExitReason.ShouldBeNull();
    }

    [Test]
    public void GivenExitDateEqualToJoinDate_WhenRecordingExit_ThenShouldSucceed()
    {
        var history = new GroupMembershipHistory(ValidMemberId, ValidJoinDate);

        history.RecordExit(ValidJoinDate);

        history.ExitedAt.ShouldBe(ValidJoinDate);
    }

    [Test]
    public void GivenExitDateBeforeJoinDate_WhenRecordingExit_ThenShouldThrowArgumentException()
    {
        var history = new GroupMembershipHistory(ValidMemberId, ValidJoinDate);
        var exitDateBeforeJoin = ValidJoinDate.AddDays(-1);

        var act = () => history.RecordExit(exitDateBeforeJoin);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenAlreadyExited_WhenRecordingExitAgain_ThenShouldThrowInvalidOperationException()
    {
        var history = new GroupMembershipHistory(ValidMemberId, ValidJoinDate);
        history.RecordExit(ValidJoinDate.AddMonths(1));

        var act = () => history.RecordExit(ValidJoinDate.AddMonths(2));

        act.ShouldThrow<InvalidOperationException>();
    }

    [Test]
    public void GivenExitReasonWithLeadingSpaces_WhenRecordingExit_ThenExitReasonShouldBeTrimmed()
    {
        var history = new GroupMembershipHistory(ValidMemberId, ValidJoinDate);

        history.RecordExit(ValidJoinDate.AddDays(1), "  Отчисление  ");

        history.ExitReason.ShouldBe("Отчисление");
    }
}
