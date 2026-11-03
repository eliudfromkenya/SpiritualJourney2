namespace SpiritualJourney.Models;

/// <summary>
/// Represents a single verse within a bible chapter.
/// </summary>
public class BibleVerse : BaseEntity
{
    /// <summary>Gets or sets the foreign key to the parent chapter.</summary>
    public string ChapterId { get; set; } = string.Empty;

    /// <summary>Gets or sets the parent chapter.</summary>
    public BibleChapter? Chapter { get; set; }

    /// <summary>Gets or sets the verse number.</summary>
    public int Number { get; set; }

    /// <summary>Gets or sets the verse text content.</summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>Gets or sets the notes attached to this verse.</summary>
    public List<VerseNote> Notes { get; set; } = [];

    /// <summary>Gets or sets the highlight applied to this verse.</summary>
    public VerseHighlight? Highlight { get; set; }
}
