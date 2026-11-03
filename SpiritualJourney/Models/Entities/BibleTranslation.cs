namespace SpiritualJourney.Models;

/// <summary>
/// Represents a bible translation stored in the local encrypted database.
/// </summary>
public class BibleTranslation : BaseEntity
{
    /// <summary>Gets or sets the display name of the translation (e.g. KJV, NIV).</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets the short code of the translation (e.g. KJV).</summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>Gets or sets the language of the translation.</summary>
    public string Language { get; set; } = string.Empty;

    /// <summary>Gets or sets the books belonging to this translation.</summary>
    public List<BibleBook> Books { get; set; } = [];
}
