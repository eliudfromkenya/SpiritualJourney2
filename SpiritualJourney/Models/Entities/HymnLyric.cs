namespace SpiritualJourney.Models;

/// <summary>
/// Represents a lyric version or translation belonging to a hymn.
/// </summary>
public class HymnLyric : BaseEntity
{
    /// <summary>Gets or sets the foreign key to the parent hymn.</summary>
    public string HymnId { get; set; } = string.Empty;

    /// <summary>Gets or sets the parent hymn.</summary>
    public Hymn? Hymn { get; set; }

    /// <summary>Gets or sets the language of this lyric version.</summary>
    public string Language { get; set; } = string.Empty;

    /// <summary>Gets or sets the lyric text content.</summary>
    public string Content { get; set; } = string.Empty;
}
