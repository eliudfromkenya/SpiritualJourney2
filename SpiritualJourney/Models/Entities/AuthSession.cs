namespace SpiritualJourney.Models;

/// <summary>
/// Represents a persisted authentication session for restoring login on relaunch.
/// </summary>
public class AuthSession : BaseEntity
{
    /// <summary>Gets or sets the foreign key to the user.</summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>Gets or sets the parent user.</summary>
    public AppUser? User { get; set; }

    /// <summary>Gets or sets the session token.</summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>Gets or sets the UTC expiry of the session.</summary>
    public DateTime ExpiresAtUtc { get; set; }
}
