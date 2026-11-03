using SpiritualJourney.Core.Enums;

namespace SpiritualJourney.Models;

/// <summary>
/// Represents a color highlight applied to a verse.
/// </summary>
public class VerseHighlight : BaseEntity
{
    /// <summary>Gets or sets the foreign key to the verse.</summary>
    public string VerseId { get; set; } = string.Empty;

    /// <summary>Gets or sets the parent verse.</summary>
    public BibleVerse? Verse { get; set; }

    /// <summary>Gets or sets the highlight color.</summary>
    public HighlightColor Color { get; set; } = HighlightColor.Yellow;
}
