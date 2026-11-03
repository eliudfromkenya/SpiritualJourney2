namespace SpiritualJourney.Models;

/// <summary>
/// Represents a sermon entry with title, content, and metadata.
/// </summary>
public class Sermon : BaseEntity
{
    /// <summary>Gets or sets the sermon title.</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Gets or sets the sermon content.</summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>Gets or sets the delivery date.</summary>
    public DateTime? DeliveryDate { get; set; }

    /// <summary>Gets or sets comma-separated tags.</summary>
    public string Tags { get; set; } = string.Empty;

    /// <summary>Gets or sets the sermon translations.</summary>
    public List<SermonTranslation> Translations { get; set; } = [];

    /// <summary>Gets or sets the venues where this sermon has been preached.</summary>
    public List<SermonVenue> Venues { get; set; } = [];
}
