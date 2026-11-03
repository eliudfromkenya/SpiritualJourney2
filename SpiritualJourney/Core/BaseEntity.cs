namespace SpiritualJourney.Core;

/// <summary>
/// Base class for all database entities. All entity identifiers are strings.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Gets or sets the unique string identifier for the entity.
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    /// <summary>
    /// Gets or sets the UTC creation timestamp.
    /// </summary>
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the UTC last-updated timestamp.
    /// </summary>
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
}
