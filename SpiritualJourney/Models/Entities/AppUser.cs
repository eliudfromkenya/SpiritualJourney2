using SpiritualJourney.Core.Enums;

namespace SpiritualJourney.Models;

/// <summary>
/// Represents an authenticated user of the application.
/// </summary>
public class AppUser : BaseEntity
{
    /// <summary>Gets or sets the user's display name.</summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>Gets or sets the user's email address.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Gets or sets the authentication provider used.</summary>
    public AuthProvider Provider { get; set; } = AuthProvider.Email;

    /// <summary>Gets or sets the provider-specific user identifier.</summary>
    public string ProviderUserId { get; set; } = string.Empty;

    /// <summary>Gets or sets the hashed password (email auth only).</summary>
    public string PasswordHash { get; set; } = string.Empty;
}
