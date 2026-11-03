namespace SpiritualJourney.Models;

/// <summary>
/// Represents the link between a sermon and a venue where it was preached.
/// </summary>
public class SermonVenue : BaseEntity
{
    /// <summary>Gets or sets the foreign key to the sermon.</summary>
    public string SermonId { get; set; } = string.Empty;

    /// <summary>Gets or sets the parent sermon.</summary>
    public Sermon? Sermon { get; set; }

    /// <summary>Gets or sets the foreign key to the venue.</summary>
    public string VenueId { get; set; } = string.Empty;

    /// <summary>Gets or sets the linked venue.</summary>
    public Venue? Venue { get; set; }

    /// <summary>Gets or sets the date the sermon was delivered at this venue.</summary>
    public DateTime DeliveredOn { get; set; }
}
