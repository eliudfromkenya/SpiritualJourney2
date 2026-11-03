namespace SpiritualJourney.Models;

/// <summary>
/// Represents a prayer sample with category and tags.
/// </summary>
public class Prayer : BaseEntity
{
    /// <summary>Gets or sets the prayer title.</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Gets or sets the prayer content text.</summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>Gets or sets the prayer category.</summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>Gets or sets comma-separated tags for searching.</summary>
    public string Tags { get; set; } = string.Empty;
}
