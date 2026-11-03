using SpiritualJourney.Core.Enums;

namespace SpiritualJourney.Models;

/// <summary>
/// Represents a user note attached to one or more verses.
/// </summary>
public class VerseNote : BaseEntity
{
    /// <summary>Gets or sets the foreign key to the verse.</summary>
    public string VerseId { get; set; } = string.Empty;

    /// <summary>Gets or sets the parent verse.</summary>
    public BibleVerse? Verse { get; set; }

    /// <summary>Gets or sets the note text.</summary>
    public string Text { get; set; } = string.Empty;
}
