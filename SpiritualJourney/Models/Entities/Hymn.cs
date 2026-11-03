namespace SpiritualJourney.Models;

/// <summary>
/// Represents a hymn with metadata and one or more lyric versions.
/// </summary>
public class Hymn : BaseEntity
{
    /// <summary>Gets or sets the hymn title.</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Gets or sets the hymn author.</summary>
    public string Author { get; set; } = string.Empty;

    /// <summary>Gets or sets comma-separated tags for searching.</summary>
    public string Tags { get; set; } = string.Empty;

    /// <summary>Gets or sets the lyric versions or translations.</summary>
    public List<HymnLyric> Lyrics { get; set; } = [];
}
