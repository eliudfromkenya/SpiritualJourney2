namespace SpiritualJourney.Models;

/// <summary>
/// Represents a single data point for a dashboard chart.
/// </summary>
public class DashboardChartItem
{
    /// <summary>Gets or sets the label for this data point.</summary>
    public string Label { get; set; } = string.Empty;

    /// <summary>Gets or sets the numeric value for this data point.</summary>
    public int Value { get; set; }

    /// <summary>Initializes a new instance of the <see cref="DashboardChartItem"/> class.</summary>
    public DashboardChartItem() { }

    /// <summary>Initializes a new instance of the <see cref="DashboardChartItem"/> class.</summary>
    public DashboardChartItem(string label, int value)
    {
        Label = label;
        Value = value;
    }
}
