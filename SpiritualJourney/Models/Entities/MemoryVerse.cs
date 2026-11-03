namespace SpiritualJourney.Models;

/// <summary>
/// Represents a verse saved by the user for memorization with spaced-repetition tracking.
/// </summary>
public class MemoryVerse : BaseEntity
{
    /// <summary>Gets or sets the foreign key to the verse being memorized.</summary>
    public string VerseId { get; set; } = string.Empty;

    /// <summary>Gets or sets the parent verse.</summary>
    public BibleVerse? Verse { get; set; }

    /// <summary>Gets or sets the foreign key to the user.</summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>Gets or sets the number of times this verse has been reviewed.</summary>
    public int Repetitions { get; set; }

    /// <summary>Gets or sets the spaced-repetition interval in days.</summary>
    public int Interval { get; set; } = 1;

    /// <summary>Gets or sets the ease factor used by the SM-2 algorithm.</summary>
    public double EaseFactor { get; set; } = 2.5;

    /// <summary>Gets or sets the next review date in UTC.</summary>
    public DateTime NextReviewAtUtc { get; set; } = DateTime.UtcNow;
}
