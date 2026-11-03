namespace SpiritualJourney.Models;

/// <summary>
/// Aggregated dashboard data containing KPIs and chart data from all modules.
/// </summary>
public class DashboardData
{
    /// <summary>Gets or sets the total number of bible translations.</summary>
    public int TranslationCount { get; set; }

    /// <summary>Gets or sets the total number of verses in the database.</summary>
    public int VerseCount { get; set; }

    /// <summary>Gets or sets the number of memory verses due for review.</summary>
    public int MemoryDueCount { get; set; }

    /// <summary>Gets or sets the total number of memory verses.</summary>
    public int MemoryTotalCount { get; set; }

    /// <summary>Gets or sets the current memory review streak in days.</summary>
    public int MemoryStreak { get; set; }

    /// <summary>Gets or sets the number of open tasks.</summary>
    public int OpenTaskCount { get; set; }

    /// <summary>Gets or sets the number of completed tasks.</summary>
    public int CompletedTaskCount { get; set; }

    /// <summary>Gets or sets the total number of sermons.</summary>
    public int SermonCount { get; set; }

    /// <summary>Gets or sets the total number of hymns.</summary>
    public int HymnCount { get; set; }

    /// <summary>Gets or sets the total number of prayers.</summary>
    public int PrayerCount { get; set; }

    /// <summary>Gets or sets the task status distribution for charting.</summary>
    public List<DashboardChartItem> TaskStatusDistribution { get; set; } = [];

    /// <summary>Gets or sets the sermon delivery trend for charting.</summary>
    public List<DashboardChartItem> SermonDeliveryTrend { get; set; } = [];
}
