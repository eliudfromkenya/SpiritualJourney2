namespace SpiritualJourney.Core.Enums;

/// <summary>
/// Represents the filter applied to the task list.
/// </summary>
public enum TaskFilter
{
    /// <summary>Show all tasks.</summary>
    All,

    /// <summary>Show tasks due today.</summary>
    Today,

    /// <summary>Show tasks due this week.</summary>
    Week,

    /// <summary>Show overdue tasks.</summary>
    Overdue
}
