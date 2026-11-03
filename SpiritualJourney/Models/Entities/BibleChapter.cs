namespace SpiritualJourney.Models;

/// <summary>
/// Represents a chapter within a bible book.
/// </summary>
public class BibleChapter : BaseEntity
{
    /// <summary>Gets or sets the foreign key to the parent book.</summary>
    public string BookId { get; set; } = string.Empty;

    /// <summary>Gets or sets the parent book.</summary>
    public BibleBook? Book { get; set; }

    /// <summary>Gets or sets the chapter number.</summary>
    public int Number { get; set; }

    /// <summary>Gets or sets the verses in this chapter.</summary>
    public List<BibleVerse> Verses { get; set; } = [];
}
