namespace SpiritualJourney.Models;

/// <summary>
/// Represents a single full-text search result from the bible.
/// </summary>
public class BibleSearchResult
{
    /// <summary>Gets or sets the verse identifier.</summary>
    public string VerseId { get; set; } = string.Empty;

    /// <summary>Gets or sets the translation code.</summary>
    public string TranslationCode { get; set; } = string.Empty;

    /// <summary>Gets or sets the book name.</summary>
    public string BookName { get; set; } = string.Empty;

    /// <summary>Gets or sets the chapter number.</summary>
    public int Chapter { get; set; }

    /// <summary>Gets or sets the verse number.</summary>
    public int Verse { get; set; }

    /// <summary>Gets or sets the verse text snippet.</summary>
    public string Snippet { get; set; } = string.Empty;

    /// <summary>Gets the formatted reference (e.g. "John 3:16").</summary>
    public string Reference => $"{BookName} {Chapter}:{Verse}";
}
