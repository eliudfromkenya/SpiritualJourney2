using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SpiritualJourney.PageModels;

/// <summary>
/// View model for the tasks page, supporting CRUD operations, filtering,
/// and completion toggling for spiritual tasks.
/// </summary>
public partial class TasksPageModel : ViewModelBase
{
    private const int WeekWindowDays = 6;
    private readonly IRepository<SpiritualTask> _taskRepository;
    private List<SpiritualTask> _allTasks = [];

    /// <summary>Gets or sets the tasks displayed in the filtered list.</summary>
    [ObservableProperty]
    private List<SpiritualTask> _tasks = [];

    /// <summary>Gets or sets the currently selected task.</summary>
    [ObservableProperty]
    private SpiritualTask? _selectedTask;

    /// <summary>Gets or sets the active filter applied to the task list.</summary>
    [ObservableProperty]
    private TaskFilter _activeFilter = TaskFilter.All;

    /// <summary>Gets or sets the title for a new task.</summary>
    [ObservableProperty]
    private string _title = string.Empty;

    /// <summary>Gets or sets the description for a new task.</summary>
    [ObservableProperty]
    private string _description = string.Empty;

    /// <summary>Gets or sets the category for a new task.</summary>
    [ObservableProperty]
    private string _category = string.Empty;

    /// <summary>Gets or sets the priority for a new task.</summary>
    [ObservableProperty]
    private TaskPriority _selectedPriority = TaskPriority.Medium;

    /// <summary>Gets or sets the due date for a new task.</summary>
    [ObservableProperty]
    private DateTime? _dueDate;

    /// <summary>
    /// Initializes a new instance of the <see cref="TasksPageModel"/> class.
    /// </summary>
    /// <param name="taskRepository">The repository for spiritual task entities.</param>
    /// <param name="errorHandler">The error handler for surfacing exceptions.</param>
    public TasksPageModel(IRepository<SpiritualTask> taskRepository, IErrorHandler errorHandler)
        : base(errorHandler)
    {
        _taskRepository = taskRepository;
    }

    /// <summary>Loads all tasks when the page appears.</summary>
    [RelayCommand]
    private async Task AppearingAsync()
    {
        await RunBusyAsync(LoadTasksAsync).ConfigureAwait(false);
    }

    /// <summary>Reapplies the active filter to the task list.</summary>
    [RelayCommand]
    private void FilterChanged()
    {
        ApplyFilter();
    }

    /// <summary>Saves a new task from the form fields and reloads the list.</summary>
    [RelayCommand]
    private async Task SaveTaskAsync()
    {
        if (string.IsNullOrWhiteSpace(Title))
        {
            return;
        }
        var task = BuildTask();
        await RunBusyAsync(async () =>
        {
            await _taskRepository.SaveAsync(task).ConfigureAwait(false);
            await LoadTasksAsync().ConfigureAwait(false);
            ResetForm();
        }).ConfigureAwait(false);
    }

    /// <summary>Deletes the selected task and reloads the list.</summary>
    [RelayCommand]
    private async Task DeleteTaskAsync()
    {
        if (SelectedTask is null)
        {
            return;
        }
        var task = SelectedTask;
        await RunBusyAsync(async () =>
        {
            await _taskRepository.DeleteAsync(task).ConfigureAwait(false);
            await LoadTasksAsync().ConfigureAwait(false);
        }).ConfigureAwait(false);
    }

    /// <summary>Toggles the completion status of the selected task.</summary>
    [RelayCommand]
    private async Task ToggleCompleteAsync()
    {
        if (SelectedTask is null)
        {
            return;
        }
        var task = SelectedTask;
        await RunBusyAsync(async () =>
        {
            task.Status = task.IsCompleted ? TaskStatus.Backlog : TaskStatus.Done;
            await _taskRepository.SaveAsync(task).ConfigureAwait(false);
            await LoadTasksAsync().ConfigureAwait(false);
        }).ConfigureAwait(false);
    }

    private async Task LoadTasksAsync()
    {
        _allTasks = await _taskRepository.ListAsync().ConfigureAwait(false);
        ApplyFilter();
    }

    private void ApplyFilter()
    {
        Tasks = ActiveFilter switch
        {
            TaskFilter.Today => _allTasks.Where(IsDueToday).ToList(),
            TaskFilter.Week => _allTasks.Where(IsDueThisWeek).ToList(),
            TaskFilter.Overdue => _allTasks.Where(IsOverdue).ToList(),
            _ => _allTasks.ToList()
        };
    }

    private static bool IsDueToday(SpiritualTask task)
        => task.DueDate is DateTime due && due.Date == DateTime.Today;

    private static bool IsDueThisWeek(SpiritualTask task)
    {
        if (task.DueDate is not DateTime due)
        {
            return false;
        }
        return due.Date >= DateTime.Today && due.Date <= DateTime.Today.AddDays(WeekWindowDays);
    }

    private static bool IsOverdue(SpiritualTask task)
        => task.DueDate is DateTime due && due.Date < DateTime.Today && !task.IsCompleted;

    private SpiritualTask BuildTask()
    {
        return new SpiritualTask
        {
            Title = Title,
            Description = Description,
            Category = Category,
            Priority = SelectedPriority,
            DueDate = DueDate,
            Status = TaskStatus.Backlog
        };
    }

    private void ResetForm()
    {
        Title = string.Empty;
        Description = string.Empty;
        Category = string.Empty;
        SelectedPriority = TaskPriority.Medium;
        DueDate = null;
    }
}
