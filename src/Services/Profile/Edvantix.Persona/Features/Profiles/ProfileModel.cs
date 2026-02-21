using Edvantix.Constants.Other;

namespace Edvantix.Persona.Features.Profiles;

public sealed class ProfileModel
{
    public Guid AccountId { get; set; }
    public string Login { get; set; } = null!;
    public Gender Gender { get; set; }

    public DateOnly BirthDate { get; set; }

    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? MiddleName { get; set; }

    public IFormFile? Avatar { get; set; } = null!;

    public string? AvatarUrl { get; set; } = null!;

    [JsonIgnore]
    public string FullName => $"{LastName} {FirstName} {MiddleName}".TrimEnd();
}
