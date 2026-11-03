namespace SpiritualJourney.Models;

/// <summary>
/// Represents a translation of a sermon for multilingual content.
/// </summary>
public class SermonTranslation : BaseEntity
{
    /// <summary>Gets or sets the foreign key to the parent sermon.</summary>
    public string SermonId { get; set; } = string.Empty;

    /// <summary>Gets or sets the parent sermon.</summary>
    public Sermon? Sermon { get; set; }

    /// <summary>Gets or sets the language of this translation.</summary>
    public string Language { get; set; } = string.Empty;

    /// <summary>Gets or sets the translated title.</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Gets or sets the translated content.</summary>
    public string Content { get; set; } = string.Empty;
}
