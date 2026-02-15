using Edvantix.Company.Domain.AggregatesModel.InvitationAggregate;
using Edvantix.Company.Domain.AggregatesModel.OrganizationMemberAggregate;

namespace Edvantix.Company.Features.InvitationFeature.Models;

/// <summary>
/// Модель ответа для приглашения.
/// </summary>
public sealed class InvitationModel
{
    public Guid Id { get; set; }
    public long OrganizationId { get; set; }
    public string? OrganizationName { get; set; }
    public long InvitedByProfileId { get; set; }
    public long? InviteeProfileId { get; set; }
    public string? InviteeEmail { get; set; }
    public OrganizationRole Role { get; set; }
    public InvitationStatus Status { get; set; }
    public Guid Token { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime? RespondedAt { get; set; }
}
