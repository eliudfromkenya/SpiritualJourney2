using SpiritualJourney.Core.Enums;

namespace SpiritualJourney.Models;

/// <summary>
/// Represents a spiritual task tracked by the user.
/// </summary>
public class SpiritualTask : BaseEntity
{
    /// <summary>Gets or sets the task title.</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Gets or sets the task description.</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>Gets or sets the task status.</summary>
    public TaskStatus Status { get; set; } = TaskStatus.Backlog;

    /// <summary>Gets or sets the task priority.</summary>
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;

    /// <summary>Gets or sets the task category.</summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>Gets or sets the due date (local time).</summary>
    public DateTime? DueDate { get; set; }

    /// <summary>Gets or sets a value indicating whether the task is completed.</summary>
    public bool IsCompleted => Status == TaskStatus.Done;
}
