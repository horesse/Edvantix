using Edvantix.Organizational.Domain.AggregatesModel.InvitationAggregate;

namespace Edvantix.Organizational.Features.InvitationFeature.Models;

/// <summary>
/// Модель ответа для приглашения.
/// </summary>
public sealed class InvitationModel
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public string? OrganizationName { get; set; }
    public Guid InvitedByProfileId { get; set; }
    public Guid? InviteeProfileId { get; set; }
    public string? InviteeEmail { get; set; }
    public OrganizationRole Role { get; set; }
    public InvitationStatus Status { get; set; }
    public Guid Token { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime? RespondedAt { get; set; }
}
