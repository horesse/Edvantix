namespace Edvantix.Organizational.Domain.AggregatesModel.InvitationAggregate;

/// <summary>
/// Статус приглашения в организацию.
/// </summary>
public enum InvitationStatus
{
    Pending = 1,
    Accepted = 2,
    Declined = 3,
    Cancelled = 4,
    Expired = 5,
}
