namespace SpiritualJourney.Models;

/// <summary>
/// Represents a book of the bible belonging to a translation.
/// </summary>
public class BibleBook : BaseEntity
{
    /// <summary>Gets or sets the foreign key to the parent translation.</summary>
    public string TranslationId { get; set; } = string.Empty;

    /// <summary>Gets or sets the parent translation.</summary>
    public BibleTranslation? Translation { get; set; }

    /// <summary>Gets or sets the name of the book (e.g. Genesis).</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets the abbreviated name used for autocomplete (e.g. Gen).</summary>
    public string ShortName { get; set; } = string.Empty;

    /// <summary>Gets or sets the ordinal position of the book.</summary>
    public int Order { get; set; }

    /// <summary>Gets or sets the testament (Old/New).</summary>
    public string Testament { get; set; } = string.Empty;

    /// <summary>Gets or sets the chapters in this book.</summary>
    public List<BibleChapter> Chapters { get; set; } = [];
}
