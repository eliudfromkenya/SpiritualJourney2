namespace SpiritualJourney.Models;

/// <summary>
/// Represents a venue or location where sermons are preached.
/// </summary>
public class Venue : BaseEntity
{
    /// <summary>Gets or sets the venue name.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets the venue address or location description.</summary>
    public string Address { get; set; } = string.Empty;

    /// <summary>Gets or sets the city.</summary>
    public string City { get; set; } = string.Empty;

    /// <summary>Gets or sets the country.</summary>
    public string Country { get; set; } = string.Empty;
}
